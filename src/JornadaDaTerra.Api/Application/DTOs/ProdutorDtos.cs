using System.ComponentModel.DataAnnotations;

namespace JornadaDaTerra.Api.Application.DTOs;

public record ProdutorDto(
    int Id,
    string Nome,
    string Email,
    int Pontos,
    int Nivel,
    DateTime CriadoEm,
    int QuantidadeFazendas);

public class CreateProdutorDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(180)]
    public string Email { get; set; } = string.Empty;
}

public class UpdateProdutorDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(120, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(180)]
    public string Email { get; set; } = string.Empty;
}
