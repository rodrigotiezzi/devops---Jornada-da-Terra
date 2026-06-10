using System.ComponentModel.DataAnnotations;
using JornadaDaTerra.Api.Domain.Enums;

namespace JornadaDaTerra.Api.Application.DTOs;

public record LeituraSateliteDto(
    int Id,
    DateTime DataLeitura,
    double TemperaturaC,
    double UmidadeRelativa,
    double Ndvi,
    double PrecipitacaoMm,
    NivelRisco RiscoCalculado,
    int SetorId);

/// <summary>
/// Entrada de uma nova leitura de satélite. O risco e a eventual missão associada
/// são calculados automaticamente pelo serviço (regra de gamificação).
/// </summary>
public class CreateLeituraSateliteDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe um setor válido.")]
    public int SetorId { get; set; }

    public DateTime? DataLeitura { get; set; }

    [Range(-60, 60, ErrorMessage = "Temperatura fora da faixa esperada (-60 a 60 °C).")]
    public double TemperaturaC { get; set; }

    [Range(0, 100, ErrorMessage = "A umidade deve estar entre 0 e 100%.")]
    public double UmidadeRelativa { get; set; }

    [Range(-1, 1, ErrorMessage = "O NDVI deve estar entre -1 e 1.")]
    public double Ndvi { get; set; }

    [Range(0, 1000, ErrorMessage = "Precipitação inválida.")]
    public double PrecipitacaoMm { get; set; }
}
