using System.ComponentModel.DataAnnotations;

namespace JornadaDaTerra.Api.Application.DTOs;

public record FazendaDto(
    int Id,
    string Nome,
    string Municipio,
    string Estado,
    double AreaHectares,
    double Latitude,
    double Longitude,
    int ProdutorId,
    int QuantidadeSetores);

public class CreateFazendaDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(120, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O município é obrigatório.")]
    [StringLength(120)]
    public string Municipio { get; set; } = string.Empty;

    [Required(ErrorMessage = "O estado (UF) é obrigatório.")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter 2 caracteres.")]
    public string Estado { get; set; } = string.Empty;

    [Range(0.1, 1_000_000, ErrorMessage = "A área deve ser maior que zero.")]
    public double AreaHectares { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude inválida.")]
    public double Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude inválida.")]
    public double Longitude { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe um produtor válido.")]
    public int ProdutorId { get; set; }
}

public class UpdateFazendaDto
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Municipio { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string Estado { get; set; } = string.Empty;

    [Range(0.1, 1_000_000)]
    public double AreaHectares { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
}
