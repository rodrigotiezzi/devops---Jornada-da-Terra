using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Domain.Entities;
using JornadaDaTerra.Api.Domain.Enums;

namespace JornadaDaTerra.Api.Application.Mapping;

/// <summary>
/// Mapeamentos explícitos entre entidades e DTOs. Optou-se por mapeamento manual
/// (em vez de AutoMapper) para manter o fluxo de dados transparente e sem "mágica".
/// </summary>
public static class MappingExtensions
{
    public static ProdutorDto ToDto(this Produtor p) => new(
        p.Id, p.Nome, p.Email, p.Pontos, p.Nivel, p.CriadoEm,
        p.Fazendas?.Count ?? 0);

    public static FazendaDto ToDto(this Fazenda f) => new(
        f.Id, f.Nome, f.Municipio, f.Estado, f.AreaHectares,
        f.Latitude, f.Longitude, f.ProdutorId,
        f.Setores?.Count ?? 0);

    public static SetorDto ToDto(this Setor s) => new(
        s.Id, s.Nome, s.Cultura, s.AreaHectares, s.FazendaId,
        s.Missoes?.Count(m => m.Status == StatusMissao.Pendente) ?? 0);

    public static LeituraSateliteDto ToDto(this LeituraSatelite l) => new(
        l.Id, l.DataLeitura, l.TemperaturaC, l.UmidadeRelativa,
        l.Ndvi, l.PrecipitacaoMm, l.RiscoCalculado, l.SetorId);

    public static MissaoDto ToDto(this Missao m) => new(
        m.Id, m.Titulo, m.Descricao, m.Tipo, m.Prioridade, m.Status,
        m.RecompensaPontos, m.CriadaEm, m.PrazoEm, m.ConcluidaEm,
        m.SetorId, m.LeituraSateliteId);

    public static Produtor ToEntity(this CreateProdutorDto dto) => new()
    {
        Nome = dto.Nome.Trim(),
        Email = dto.Email.Trim().ToLowerInvariant(),
        Pontos = 0,
        Nivel = 1,
        CriadoEm = DateTime.UtcNow
    };

    public static Fazenda ToEntity(this CreateFazendaDto dto) => new()
    {
        Nome = dto.Nome.Trim(),
        Municipio = dto.Municipio.Trim(),
        Estado = dto.Estado.Trim().ToUpperInvariant(),
        AreaHectares = dto.AreaHectares,
        Latitude = dto.Latitude,
        Longitude = dto.Longitude,
        ProdutorId = dto.ProdutorId
    };

    public static Setor ToEntity(this CreateSetorDto dto) => new()
    {
        Nome = dto.Nome.Trim(),
        Cultura = dto.Cultura.Trim(),
        AreaHectares = dto.AreaHectares,
        FazendaId = dto.FazendaId
    };

    public static Missao ToEntity(this CreateMissaoDto dto) => new()
    {
        Titulo = dto.Titulo.Trim(),
        Descricao = dto.Descricao.Trim(),
        Tipo = dto.Tipo,
        Prioridade = dto.Prioridade,
        Status = StatusMissao.Pendente,
        RecompensaPontos = dto.RecompensaPontos,
        PrazoEm = dto.PrazoEm,
        CriadaEm = DateTime.UtcNow,
        SetorId = dto.SetorId
    };
}
