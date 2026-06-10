namespace JornadaDaTerra.Api.Application.Common;

/// <summary>Envelope de resposta paginada para listagens da API.</summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Itens { get; init; } = Array.Empty<T>();
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
    public int TotalItens { get; init; }
    public int TotalPaginas => TamanhoPagina == 0 ? 0 : (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);

    public PagedResult() { }

    public PagedResult(IReadOnlyList<T> itens, int pagina, int tamanhoPagina, int totalItens)
    {
        Itens = itens;
        Pagina = pagina;
        TamanhoPagina = tamanhoPagina;
        TotalItens = totalItens;
    }
}

/// <summary>Parâmetros de paginação reutilizáveis nas listagens.</summary>
public class QueryParameters
{
    private const int MaxTamanhoPagina = 100;
    private int _tamanhoPagina = 20;
    private int _pagina = 1;

    public int Pagina
    {
        get => _pagina;
        set => _pagina = value < 1 ? 1 : value;
    }

    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        set => _tamanhoPagina = value is < 1 or > MaxTamanhoPagina ? 20 : value;
    }

    public int Skip => (Pagina - 1) * TamanhoPagina;
}
