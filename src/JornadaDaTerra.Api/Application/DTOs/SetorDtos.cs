using System.ComponentModel.DataAnnotations;

namespace JornadaDaTerra.Api.Application.DTOs;

public record SetorDto(
    int Id,
    string Nome,
    string Cultura,
    double AreaHectares,
    int FazendaId,
    int QuantidadeMissoesPendentes);

public class CreateSetorDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(120, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cultura é obrigatória.")]
    [StringLength(80)]
    public string Cultura { get; set; } = string.Empty;

    [Range(0.1, 1_000_000, ErrorMessage = "A área deve ser maior que zero.")]
    public double AreaHectares { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe uma fazenda válida.")]
    public int FazendaId { get; set; }
}

public class UpdateSetorDto
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Cultura { get; set; } = string.Empty;

    [Range(0.1, 1_000_000)]
    public double AreaHectares { get; set; }
}
