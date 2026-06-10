using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Mapping;
using JornadaDaTerra.Api.Domain.Entities;
using JornadaDaTerra.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JornadaDaTerra.Api.Application.Services;

public class FazendaService : IFazendaService
{
    private readonly IRepository<Fazenda> _repo;
    private readonly IRepository<Produtor> _produtores;

    public FazendaService(IRepository<Fazenda> repo, IRepository<Produtor> produtores)
    {
        _repo = repo;
        _produtores = produtores;
    }

    public async Task<PagedResult<FazendaDto>> ListarAsync(QueryParameters q, int? produtorId, CancellationToken ct = default)
    {
        var total = await _repo.CountAsync(
            f => produtorId == null || f.ProdutorId == produtorId, ct);

        var itens = await _repo.GetAllAsync(
            filter: f => produtorId == null || f.ProdutorId == produtorId,
            include: query => query.Include(f => f.Setores).OrderBy(f => f.Nome),
            skip: q.Skip, take: q.TamanhoPagina, cancellationToken: ct);

        return new PagedResult<FazendaDto>(
            itens.Select(f => f.ToDto()).ToList(), q.Pagina, q.TamanhoPagina, total);
    }

    public async Task<FazendaDto> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var fazenda = await _repo.GetByIdAsync(id,
            include: query => query.Include(f => f.Setores), cancellationToken: ct)
            ?? throw NotFoundException.Para("Fazenda", id);
        return fazenda.ToDto();
    }

    public async Task<FazendaDto> CriarAsync(CreateFazendaDto dto, CancellationToken ct = default)
    {
        if (!await _produtores.ExistsAsync(p => p.Id == dto.ProdutorId, ct))
            throw new RegraDeNegocioException($"Produtor {dto.ProdutorId} não existe.");

        var fazenda = dto.ToEntity();
        await _repo.AddAsync(fazenda, ct);
        await _repo.SaveChangesAsync(ct);
        return fazenda.ToDto();
    }

    public async Task<FazendaDto> AtualizarAsync(int id, UpdateFazendaDto dto, CancellationToken ct = default)
    {
        var fazenda = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Fazenda", id);

        fazenda.Nome = dto.Nome.Trim();
        fazenda.Municipio = dto.Municipio.Trim();
        fazenda.Estado = dto.Estado.Trim().ToUpperInvariant();
        fazenda.AreaHectares = dto.AreaHectares;
        fazenda.Latitude = dto.Latitude;
        fazenda.Longitude = dto.Longitude;

        _repo.Update(fazenda);
        await _repo.SaveChangesAsync(ct);
        return fazenda.ToDto();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var fazenda = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Fazenda", id);
        _repo.Remove(fazenda);
        await _repo.SaveChangesAsync(ct);
    }
}
