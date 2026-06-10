namespace JornadaDaTerra.Api.Domain.Entities;

/// <summary>
/// Propriedade rural monitorada via satélite. Pertence a um <see cref="Produtor"/>
/// e é subdividida em setores produtivos.
/// </summary>
public class Fazenda
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Municipio { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public double AreaHectares { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    // Relacionamento N:1 — várias fazendas para um produtor.
    public int ProdutorId { get; set; }
    public Produtor? Produtor { get; set; }

    // Relacionamento 1:N — uma fazenda possui vários setores.
    public ICollection<Setor> Setores { get; set; } = new List<Setor>();
}
