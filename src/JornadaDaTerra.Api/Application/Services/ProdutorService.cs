using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Mapping;
using JornadaDaTerra.Api.Domain.Entities;
using JornadaDaTerra.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JornadaDaTerra.Api.Application.Services;

public class ProdutorService : IProdutorService
{
    private readonly IRepository<Produtor> _repo;

    public ProdutorService(IRepository<Produtor> repo) => _repo = repo;

    public async Task<PagedResult<ProdutorDto>> ListarAsync(QueryParameters q, CancellationToken ct = default)
    {
        var total = await _repo.CountAsync(cancellationToken: ct);
        var itens = await _repo.GetAllAsync(
            include: query => query.Include(p => p.Fazendas).OrderBy(p => p.Nome),
            skip: q.Skip, take: q.TamanhoPagina, cancellationToken: ct);

        return new PagedResult<ProdutorDto>(
            itens.Select(p => p.ToDto()).ToList(), q.Pagina, q.TamanhoPagina, total);
    }

    public async Task<ProdutorDto> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var produtor = await _repo.GetByIdAsync(id,
            include: query => query.Include(p => p.Fazendas), cancellationToken: ct)
            ?? throw NotFoundException.Para("Produtor", id);
        return produtor.ToDto();
    }

    public async Task<ProdutorDto> CriarAsync(CreateProdutorDto dto, CancellationToken ct = default)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await _repo.ExistsAsync(p => p.Email == email, ct))
            throw new ConflictException($"Já existe um produtor com o e-mail '{email}'.");

        var produtor = dto.ToEntity();
        await _repo.AddAsync(produtor, ct);
        await _repo.SaveChangesAsync(ct);
        return produtor.ToDto();
    }

    public async Task<ProdutorDto> AtualizarAsync(int id, UpdateProdutorDto dto, CancellationToken ct = default)
    {
        var produtor = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Produtor", id);

        var email = dto.Email.Trim().ToLowerInvariant();
        if (await _repo.ExistsAsync(p => p.Email == email && p.Id != id, ct))
            throw new ConflictException($"Já existe outro produtor com o e-mail '{email}'.");

        produtor.Nome = dto.Nome.Trim();
        produtor.Email = email;

        _repo.Update(produtor);
        await _repo.SaveChangesAsync(ct);
        return produtor.ToDto();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var produtor = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Produtor", id);
        _repo.Remove(produtor);
        await _repo.SaveChangesAsync(ct);
    }
}
