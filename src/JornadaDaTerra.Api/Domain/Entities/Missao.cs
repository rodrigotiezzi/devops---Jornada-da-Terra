using JornadaDaTerra.Api.Domain.Enums;

namespace JornadaDaTerra.Api.Domain.Entities;

/// <summary>
/// "Missão" gamificada entregue ao produtor (ex.: "Alerta de geada no setor sul:
/// inicie a proteção da colheita"). Concluí-la recompensa o produtor com pontos.
/// </summary>
public class Missao
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public TipoEvento Tipo { get; set; }

    public PrioridadeMissao Prioridade { get; set; }

    public StatusMissao Status { get; set; } = StatusMissao.Pendente;

    /// <summary>Pontos concedidos ao produtor quando a missão é concluída.</summary>
    public int RecompensaPontos { get; set; }

    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public DateTime? PrazoEm { get; set; }

    public DateTime? ConcluidaEm { get; set; }

    // Relacionamento N:1 — várias missões para um setor.
    public int SetorId { get; set; }
    public Setor? Setor { get; set; }

    /// <summary>Leitura de satélite que originou a missão (opcional).</summary>
    public int? LeituraSateliteId { get; set; }
    public LeituraSatelite? LeituraSatelite { get; set; }
}
