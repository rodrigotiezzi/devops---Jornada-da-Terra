using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Mapping;
using JornadaDaTerra.Api.Domain.Entities;
using JornadaDaTerra.Api.Domain.Enums;
using JornadaDaTerra.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JornadaDaTerra.Api.Application.Services;

public class MissaoService : IMissaoService
{
    private const int PontosPorNivel = 100;

    private readonly IRepository<Missao> _repo;
    private readonly IRepository<Setor> _setores;
    private readonly IRepository<Produtor> _produtores;

    public MissaoService(
        IRepository<Missao> repo,
        IRepository<Setor> setores,
        IRepository<Produtor> produtores)
    {
        _repo = repo;
        _setores = setores;
        _produtores = produtores;
    }

    public async Task<PagedResult<MissaoDto>> ListarAsync(
        QueryParameters q, int? setorId, StatusMissao? status, CancellationToken ct = default)
    {
        var total = await _repo.CountAsync(
            m => (setorId == null || m.SetorId == setorId) &&
                 (status == null || m.Status == status), ct);

        var itens = await _repo.GetAllAsync(
            filter: m => (setorId == null || m.SetorId == setorId) &&
                         (status == null || m.Status == status),
            include: query => query
                .OrderByDescending(m => m.Prioridade)
                .ThenByDescending(m => m.CriadaEm),
            skip: q.Skip, take: q.TamanhoPagina, cancellationToken: ct);

        return new PagedResult<MissaoDto>(
            itens.Select(m => m.ToDto()).ToList(), q.Pagina, q.TamanhoPagina, total);
    }

    public async Task<MissaoDto> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var missao = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Missão", id);
        return missao.ToDto();
    }

    public async Task<MissaoDto> CriarAsync(CreateMissaoDto dto, CancellationToken ct = default)
    {
        if (!await _setores.ExistsAsync(s => s.Id == dto.SetorId, ct))
            throw new RegraDeNegocioException($"Setor {dto.SetorId} não existe.");

        var missao = dto.ToEntity();
        await _repo.AddAsync(missao, ct);
        await _repo.SaveChangesAsync(ct);
        return missao.ToDto();
    }

    public async Task<MissaoDto> AtualizarStatusAsync(int id, AtualizarStatusMissaoDto dto, CancellationToken ct = default)
    {
        var missao = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Missão", id);

        if (missao.Status == StatusMissao.Concluida && dto.Status != StatusMissao.Concluida)
            throw new RegraDeNegocioException("Uma missão concluída não pode mudar de status.");

        var concluindoAgora = dto.Status == StatusMissao.Concluida
            && missao.Status != StatusMissao.Concluida;

        missao.Status = dto.Status;

        if (concluindoAgora)
        {
            missao.ConcluidaEm = DateTime.UtcNow;
            await CreditarPontosAoProdutorAsync(missao, ct);
        }

        _repo.Update(missao);
        await _repo.SaveChangesAsync(ct);
        return missao.ToDto();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var missao = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Missão", id);
        _repo.Remove(missao);
        await _repo.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Concede a recompensa da missão ao produtor dono (Setor -> Fazenda -> Produtor)
    /// e recalcula o nível (1 nível a cada 100 pontos).
    /// </summary>
    private async Task CreditarPontosAoProdutorAsync(Missao missao, CancellationToken ct)
    {
        var setor = await _setores.GetByIdAsync(missao.SetorId,
            include: q => q.Include(s => s.Fazenda), cancellationToken: ct);

        if (setor?.Fazenda is null)
            return;

        var produtor = await _produtores.GetByIdAsync(setor.Fazenda.ProdutorId, cancellationToken: ct);
        if (produtor is null)
            return;

        produtor.Pontos += missao.RecompensaPontos;
        produtor.Nivel = (produtor.Pontos / PontosPorNivel) + 1;
        _produtores.Update(produtor);
    }
}
