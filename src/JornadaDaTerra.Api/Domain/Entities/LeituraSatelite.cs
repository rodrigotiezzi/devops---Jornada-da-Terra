using JornadaDaTerra.Api.Domain.Enums;

namespace JornadaDaTerra.Api.Domain.Entities;

/// <summary>
/// Registro de uma observação orbital sobre um <see cref="Setor"/>. A partir destes
/// dados o sistema calcula o risco e gera missões para o produtor.
/// </summary>
public class LeituraSatelite
{
    public int Id { get; set; }

    public DateTime DataLeitura { get; set; } = DateTime.UtcNow;

    /// <summary>Temperatura média da superfície (°C).</summary>
    public double TemperaturaC { get; set; }

    /// <summary>Umidade relativa do ar (%).</summary>
    public double UmidadeRelativa { get; set; }

    /// <summary>Índice de vegetação por diferença normalizada (NDVI), de -1 a 1.</summary>
    public double Ndvi { get; set; }

    /// <summary>Precipitação acumulada estimada (mm).</summary>
    public double PrecipitacaoMm { get; set; }

    /// <summary>Risco calculado a partir dos indicadores acima.</summary>
    public NivelRisco RiscoCalculado { get; set; } = NivelRisco.Nenhum;

    // Relacionamento N:1 — várias leituras para um setor.
    public int SetorId { get; set; }
    public Setor? Setor { get; set; }
}
