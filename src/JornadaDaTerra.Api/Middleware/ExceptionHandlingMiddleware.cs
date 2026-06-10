using System.Text.Json;
using JornadaDaTerra.Api.Application.Common;

namespace JornadaDaTerra.Api.Middleware;

/// <summary>
/// Captura exceções não tratadas e devolve uma resposta JSON padronizada (ProblemDetails),
/// mapeando as exceções de domínio para os códigos HTTP corretos.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        var (status, titulo) = ex switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
            RegraDeNegocioException => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
        };

        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(ex, "Erro não tratado ao processar {Path}", context.Request.Path);
        else
            _logger.LogWarning("{Titulo}: {Mensagem}", titulo, ex.Message);

        var problema = new
        {
            type = $"https://httpstatuses.io/{status}",
            title = titulo,
            status,
            detail = status == StatusCodes.Status500InternalServerError && !_env.IsDevelopment()
                ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
                : ex.Message,
            instance = context.Request.Path.Value,
            traceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = status;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problema, options));
    }
}

/// <summary>Extensão para registrar o middleware no pipeline.</summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
