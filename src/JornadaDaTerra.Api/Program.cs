using System.Reflection;
using System.Text.Json.Serialization;
using JornadaDaTerra.Api.Application.Services;
using JornadaDaTerra.Api.Infrastructure.Data;
using JornadaDaTerra.Api.Infrastructure.Repositories;
using JornadaDaTerra.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------------------
// 1) Banco de dados (EF Core + Oracle)
// ----------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("OracleConnection")
    ?? throw new InvalidOperationException("ConnectionString 'OracleConnection' não configurada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString));

// ----------------------------------------------------------------------------
// 2) Injeção de dependências (repositório genérico + services de aplicação)
// ----------------------------------------------------------------------------
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProdutorService, ProdutorService>();
builder.Services.AddScoped<IFazendaService, FazendaService>();
builder.Services.AddScoped<ISetorService, SetorService>();
builder.Services.AddScoped<ILeituraSateliteService, LeituraSateliteService>();
builder.Services.AddScoped<IMissaoService, MissaoService>();

// ----------------------------------------------------------------------------
// 3) MVC / Controllers (enums serializados como texto)
// ----------------------------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// ----------------------------------------------------------------------------
// 4) Swagger / OpenAPI
// ----------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Jornada da Terra API",
        Version = "v1",
        Description = "API REST do projeto Agro & Clima Gamificado (Global Solution FIAP 2026/1). " +
                      "Traduz dados de satélite em missões para o produtor rural."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// ----------------------------------------------------------------------------
// 5) Pipeline HTTP
// ----------------------------------------------------------------------------
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jornada da Terra API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz (https://localhost:porta/)
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// ----------------------------------------------------------------------------
// 6) Migrations automáticas + seed (apenas em Desenvolvimento)
// ----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("Startup");
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var leituraService = scope.ServiceProvider.GetRequiredService<ILeituraSateliteService>();
        await DbSeeder.SeedAsync(db, leituraService);

        logger.LogInformation("Banco de dados migrado e populado com sucesso.");
    }
    catch (Exception ex)
    {
        // Não derruba a aplicação: o Swagger continua acessível mesmo sem o banco.
        logger.LogError(ex,
            "Falha ao migrar/popular o banco. Verifique a ConnectionString 'OracleConnection' " +
            "em appsettings.json e se o Oracle está acessível.");
    }
}

app.Run();
