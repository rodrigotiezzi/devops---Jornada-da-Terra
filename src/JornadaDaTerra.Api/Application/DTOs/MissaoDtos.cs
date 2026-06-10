using System.ComponentModel.DataAnnotations;
using JornadaDaTerra.Api.Domain.Enums;

namespace JornadaDaTerra.Api.Application.DTOs;

public record MissaoDto(
    int Id,
    string Titulo,
    string Descricao,
    TipoEvento Tipo,
    PrioridadeMissao Prioridade,
    StatusMissao Status,
    int RecompensaPontos,
    DateTime CriadaEm,
    DateTime? PrazoEm,
    DateTime? ConcluidaEm,
    int SetorId,
    int? LeituraSateliteId);

/// <summary>Criação manual de uma missão (além das geradas automaticamente).</summary>
public class CreateMissaoDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150, MinimumLength = 3)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(600, MinimumLength = 5)]
    public string Descricao { get; set; } = string.Empty;

    [EnumDataType(typeof(TipoEvento), ErrorMessage = "Tipo de evento inválido.")]
    public TipoEvento Tipo { get; set; }

    [EnumDataType(typeof(PrioridadeMissao), ErrorMessage = "Prioridade inválida.")]
    public PrioridadeMissao Prioridade { get; set; }

    [Range(1, 1000, ErrorMessage = "A recompensa deve estar entre 1 e 1000 pontos.")]
    public int RecompensaPontos { get; set; }

    public DateTime? PrazoEm { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe um setor válido.")]
    public int SetorId { get; set; }
}

/// <summary>Atualiza o status de uma missão na jornada (ex.: iniciar ou concluir).</summary>
public class AtualizarStatusMissaoDto
{
    [EnumDataType(typeof(StatusMissao), ErrorMessage = "Status inválido.")]
    public StatusMissao Status { get; set; }
}
