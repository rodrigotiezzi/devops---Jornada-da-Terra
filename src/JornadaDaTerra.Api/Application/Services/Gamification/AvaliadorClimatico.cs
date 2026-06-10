using JornadaDaTerra.Api.Domain.Entities;
using JornadaDaTerra.Api.Domain.Enums;

namespace JornadaDaTerra.Api.Application.Services.Gamification;

/// <summary>Resultado da avaliação de uma leitura de satélite.</summary>
public record AvaliacaoClimatica(
    NivelRisco Risco,
    bool GerarMissao,
    TipoEvento Tipo,
    PrioridadeMissao Prioridade,
    string Titulo,
    string Descricao,
    int RecompensaPontos);

/// <summary>
/// Núcleo da gamificação: traduz indicadores orbitais (temperatura, umidade, NDVI,
/// precipitação) em um nível de risco e, quando relevante, em uma "missão" para o produtor.
/// Espelha o que, no banco Oracle, seria feito por triggers/blocos PL-SQL.
/// </summary>
public static class AvaliadorClimatico
{
    public static AvaliacaoClimatica Avaliar(LeituraSatelite l, string nomeSetor)
    {
        // 1) Geada — temperatura muito baixa é o evento mais crítico.
        if (l.TemperaturaC <= 3)
        {
            var (risco, prioridade, pontos) = l.TemperaturaC <= 0
                ? (NivelRisco.Severo, PrioridadeMissao.Critica, 100)
                : (NivelRisco.Alto, PrioridadeMissao.Alta, 70);

            return new AvaliacaoClimatica(
                risco, true, TipoEvento.Geada, prioridade,
                $"Alerta de geada no {nomeSetor}",
                $"Temperatura de {l.TemperaturaC:0.#} °C detectada. Inicie a proteção da colheita " +
                "(irrigação por aspersão ou cobertura) nas próximas horas.",
                pontos);
        }

        // 2) Excesso de chuva — risco de alagamento/erosão.
        if (l.PrecipitacaoMm >= 80)
        {
            return new AvaliacaoClimatica(
                NivelRisco.Alto, true, TipoEvento.ExcessoDeChuva, PrioridadeMissao.Alta,
                $"Excesso de chuva no {nomeSetor}",
                $"Precipitação acumulada de {l.PrecipitacaoMm:0} mm. Verifique a drenagem e " +
                "evite o tráfego de máquinas para reduzir compactação e erosão.",
                60);
        }

        // 3) Estresse hídrico / seca — baixa umidade e pouca chuva.
        if (l.UmidadeRelativa < 30 && l.PrecipitacaoMm < 5)
        {
            return new AvaliacaoClimatica(
                NivelRisco.Moderado, true, TipoEvento.EstresseHidrico, PrioridadeMissao.Media,
                $"Estresse hídrico no {nomeSetor}",
                $"Umidade de {l.UmidadeRelativa:0}% e chuva quase nula. Programe a irrigação " +
                "e monitore o solo para evitar perda de produtividade.",
                40);
        }

        // 4) Possível praga / queda de vigor — NDVI baixo sem causa climática óbvia.
        if (l.Ndvi < 0.30)
        {
            return new AvaliacaoClimatica(
                NivelRisco.Moderado, true, TipoEvento.Praga, PrioridadeMissao.Media,
                $"Queda de vigor no {nomeSetor}",
                $"NDVI de {l.Ndvi:0.00} indica baixo vigor vegetativo. Faça uma inspeção de campo " +
                "para descartar pragas, doenças ou deficiência nutricional.",
                35);
        }

        // 5) Janela ideal de colheita — NDVI alto e clima estável (missão positiva).
        if (l.Ndvi >= 0.75)
        {
            return new AvaliacaoClimatica(
                NivelRisco.Nenhum, true, TipoEvento.ColheitaIdeal, PrioridadeMissao.Baixa,
                $"Janela ideal no {nomeSetor}",
                $"NDVI de {l.Ndvi:0.00} e clima estável. Planeje a colheita para aproveitar o " +
                "pico de produtividade do setor.",
                25);
        }

        // Sem evento relevante: apenas registra a leitura com risco baixo, sem missão.
        return new AvaliacaoClimatica(
            NivelRisco.Baixo, false, TipoEvento.ColheitaIdeal, PrioridadeMissao.Baixa,
            string.Empty, string.Empty, 0);
    }
}
