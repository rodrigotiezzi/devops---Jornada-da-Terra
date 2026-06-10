using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Mapping;
using JornadaDaTerra.Api.Domain.Entities;
using JornadaDaTerra.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JornadaDaTerra.Api.Application.Services;

public class SetorService : ISetorService
{
    private readonly IRepository<Setor> _repo;
    private readonly IRepository<Fazenda> _fazendas;

    public SetorService(IRepository<Setor> repo, IRepository<Fazenda> fazendas)
    {
        _repo = repo;
        _fazendas = fazendas;
    }

    public async Task<PagedResult<SetorDto>> ListarAsync(QueryParameters q, int? fazendaId, CancellationToken ct = default)
    {
        var total = await _repo.CountAsync(
            s => fazendaId == null || s.FazendaId == fazendaId, ct);

        var itens = await _repo.GetAllAsync(
            filter: s => fazendaId == null || s.FazendaId == fazendaId,
            include: query => query.Include(s => s.Missoes).OrderBy(s => s.Nome),
            skip: q.Skip, take: q.TamanhoPagina, cancellationToken: ct);

        return new PagedResult<SetorDto>(
            itens.Select(s => s.ToDto()).ToList(), q.Pagina, q.TamanhoPagina, total);
    }

    public async Task<SetorDto> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var setor = await _repo.GetByIdAsync(id,
            include: query => query.Include(s => s.Missoes), cancellationToken: ct)
            ?? throw NotFoundException.Para("Setor", id);
        return setor.ToDto();
    }

    public async Task<SetorDto> CriarAsync(CreateSetorDto dto, CancellationToken ct = default)
    {
        if (!await _fazendas.ExistsAsync(f => f.Id == dto.FazendaId, ct))
            throw new RegraDeNegocioException($"Fazenda {dto.FazendaId} não existe.");

        var setor = dto.ToEntity();
        await _repo.AddAsync(setor, ct);
        await _repo.SaveChangesAsync(ct);
        return setor.ToDto();
    }

    public async Task<SetorDto> AtualizarAsync(int id, UpdateSetorDto dto, CancellationToken ct = default)
    {
        var setor = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Setor", id);

        setor.Nome = dto.Nome.Trim();
        setor.Cultura = dto.Cultura.Trim();
        setor.AreaHectares = dto.AreaHectares;

        _repo.Update(setor);
        await _repo.SaveChangesAsync(ct);
        return setor.ToDto();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var setor = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Setor", id);
        _repo.Remove(setor);
        await _repo.SaveChangesAsync(ct);
    }
}
