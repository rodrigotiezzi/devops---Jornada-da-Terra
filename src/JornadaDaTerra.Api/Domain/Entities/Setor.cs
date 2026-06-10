namespace JornadaDaTerra.Api.Domain.Entities;

/// <summary>
/// Subárea de uma <see cref="Fazenda"/> (ex.: "Setor Sul - Soja"). É a unidade
/// para a qual chegam leituras de satélite e onde as missões são executadas.
/// </summary>
public class Setor
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cultura { get; set; } = string.Empty;

    public double AreaHectares { get; set; }

    // Relacionamento N:1 — vários setores para uma fazenda.
    public int FazendaId { get; set; }
    public Fazenda? Fazenda { get; set; }

    // Relacionamentos 1:N.
    public ICollection<LeituraSatelite> Leituras { get; set; } = new List<LeituraSatelite>();
    public ICollection<Missao> Missoes { get; set; } = new List<Missao>();
}
