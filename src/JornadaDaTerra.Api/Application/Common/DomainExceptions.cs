namespace JornadaDaTerra.Api.Application.Common;

/// <summary>Recurso solicitado não foi encontrado (mapeado para HTTP 404).</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string mensagem) : base(mensagem) { }

    public static NotFoundException Para(string recurso, int id)
        => new($"{recurso} com Id {id} não foi encontrado(a).");
}

/// <summary>Conflito de regra de negócio, ex.: e-mail já cadastrado (mapeado para HTTP 409).</summary>
public class ConflictException : Exception
{
    public ConflictException(string mensagem) : base(mensagem) { }
}

/// <summary>Violação de regra de negócio na entrada (mapeado para HTTP 422).</summary>
public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string mensagem) : base(mensagem) { }
}
