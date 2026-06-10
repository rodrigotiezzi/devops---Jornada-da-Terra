namespace JornadaDaTerra.Api.Domain.Entities;

/// <summary>
/// Representa o produtor rural (jogador) que percorre a "Jornada da Terra".
/// Acumula pontos e nível conforme conclui missões geradas a partir dos dados de satélite.
/// </summary>
public class Produtor
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>Pontos de experiência acumulados ao concluir missões.</summary>
    public int Pontos { get; set; }

    /// <summary>Nível atual, derivado dos pontos (1 nível a cada 100 pontos).</summary>
    public int Nivel { get; set; } = 1;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Relacionamento 1:N — um produtor possui várias fazendas.
    public ICollection<Fazenda> Fazendas { get; set; } = new List<Fazenda>();
}
