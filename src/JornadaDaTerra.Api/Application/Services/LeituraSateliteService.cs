using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Mapping;
using JornadaDaTerra.Api.Application.Services.Gamification;
using JornadaDaTerra.Api.Domain.Entities;
using JornadaDaTerra.Api.Domain.Enums;
using JornadaDaTerra.Api.Infrastructure.Repositories;

namespace JornadaDaTerra.Api.Application.Services;

public class LeituraSateliteService : ILeituraSateliteService
{
    private readonly IRepository<LeituraSatelite> _repo;
    private readonly IRepository<Setor> _setores;
    private readonly IRepository<Missao> _missoes;
    private readonly ILogger<LeituraSateliteService> _logger;

    public LeituraSateliteService(
        IRepository<LeituraSatelite> repo,
        IRepository<Setor> setores,
        IRepository<Missao> missoes,
        ILogger<LeituraSateliteService> logger)
    {
        _repo = repo;
        _setores = setores;
        _missoes = missoes;
        _logger = logger;
    }

    public async Task<PagedResult<LeituraSateliteDto>> ListarAsync(QueryParameters q, int? setorId, CancellationToken ct = default)
    {
        var total = await _repo.CountAsync(l => setorId == null || l.SetorId == setorId, ct);
        var itens = await _repo.GetAllAsync(
            filter: l => setorId == null || l.SetorId == setorId,
            include: query => query.OrderByDescending(l => l.DataLeitura),
            skip: q.Skip, take: q.TamanhoPagina, cancellationToken: ct);

        return new PagedResult<LeituraSateliteDto>(
            itens.Select(l => l.ToDto()).ToList(), q.Pagina, q.TamanhoPagina, total);
    }

    public async Task<LeituraSateliteDto> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var leitura = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Leitura de satélite", id);
        return leitura.ToDto();
    }

    public async Task<LeituraSateliteDto> RegistrarAsync(CreateLeituraSateliteDto dto, CancellationToken ct = default)
    {
        var setor = await _setores.GetByIdAsync(dto.SetorId, cancellationToken: ct)
            ?? throw new RegraDeNegocioException($"Setor {dto.SetorId} não existe.");

        var leitura = new LeituraSatelite
        {
            SetorId = dto.SetorId,
            DataLeitura = dto.DataLeitura ?? DateTime.UtcNow,
            TemperaturaC = dto.TemperaturaC,
            UmidadeRelativa = dto.UmidadeRelativa,
            Ndvi = dto.Ndvi,
            PrecipitacaoMm = dto.PrecipitacaoMm
        };

        // Regra de gamificação: avalia a leitura e calcula o risco.
        var avaliacao = AvaliadorClimatico.Avaliar(leitura, setor.Nome);
        leitura.RiscoCalculado = avaliacao.Risco;

        await _repo.AddAsync(leitura, ct);
        await _repo.SaveChangesAsync(ct);

        // Se a avaliação indicar, gera automaticamente a missão vinculada à leitura.
        if (avaliacao.GerarMissao)
        {
            var missao = new Missao
            {
                Titulo = avaliacao.Titulo,
                Descricao = avaliacao.Descricao,
                Tipo = avaliacao.Tipo,
                Prioridade = avaliacao.Prioridade,
                Status = StatusMissao.Pendente,
                RecompensaPontos = avaliacao.RecompensaPontos,
                CriadaEm = DateTime.UtcNow,
                PrazoEm = DateTime.UtcNow.AddDays(2),
                SetorId = setor.Id,
                LeituraSateliteId = leitura.Id
            };

            await _missoes.AddAsync(missao, ct);
            await _missoes.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Missão automática '{Titulo}' gerada para o setor {SetorId} (risco {Risco}).",
                missao.Titulo, setor.Id, leitura.RiscoCalculado);
        }

        return leitura.ToDto();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var leitura = await _repo.GetByIdAsync(id, cancellationToken: ct)
            ?? throw NotFoundException.Para("Leitura de satélite", id);
        _repo.Remove(leitura);
        await _repo.SaveChangesAsync(ct);
    }
}
