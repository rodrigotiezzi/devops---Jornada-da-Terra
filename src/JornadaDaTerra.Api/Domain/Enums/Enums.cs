namespace JornadaDaTerra.Api.Domain.Enums;

/// <summary>Tipo do evento climático/agronômico detectado pelo satélite que origina uma missão.</summary>
public enum TipoEvento
{
    Geada = 1,
    Seca = 2,
    ExcessoDeChuva = 3,
    Praga = 4,
    EstresseHidrico = 5,
    ColheitaIdeal = 6,
    QueimadaProxima = 7
}

/// <summary>Prioridade da missão, usada para ordenar o engajamento do produtor.</summary>
public enum PrioridadeMissao
{
    Baixa = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

/// <summary>Estado do ciclo de vida de uma missão na "jornada" do produtor.</summary>
public enum StatusMissao
{
    Pendente = 1,
    EmProgresso = 2,
    Concluida = 3,
    Expirada = 4
}

/// <summary>Nível de risco calculado a partir da leitura de satélite.</summary>
public enum NivelRisco
{
    Nenhum = 0,
    Baixo = 1,
    Moderado = 2,
    Alto = 3,
    Severo = 4
}
