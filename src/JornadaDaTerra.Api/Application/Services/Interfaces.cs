using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Domain.Enums;

namespace JornadaDaTerra.Api.Application.Services;

public interface IProdutorService
{
    Task<PagedResult<ProdutorDto>> ListarAsync(QueryParameters q, CancellationToken ct = default);
    Task<ProdutorDto> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<ProdutorDto> CriarAsync(CreateProdutorDto dto, CancellationToken ct = default);
    Task<ProdutorDto> AtualizarAsync(int id, UpdateProdutorDto dto, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

public interface IFazendaService
{
    Task<PagedResult<FazendaDto>> ListarAsync(QueryParameters q, int? produtorId, CancellationToken ct = default);
    Task<FazendaDto> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<FazendaDto> CriarAsync(CreateFazendaDto dto, CancellationToken ct = default);
    Task<FazendaDto> AtualizarAsync(int id, UpdateFazendaDto dto, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

public interface ISetorService
{
    Task<PagedResult<SetorDto>> ListarAsync(QueryParameters q, int? fazendaId, CancellationToken ct = default);
    Task<SetorDto> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<SetorDto> CriarAsync(CreateSetorDto dto, CancellationToken ct = default);
    Task<SetorDto> AtualizarAsync(int id, UpdateSetorDto dto, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

public interface ILeituraSateliteService
{
    Task<PagedResult<LeituraSateliteDto>> ListarAsync(QueryParameters q, int? setorId, CancellationToken ct = default);
    Task<LeituraSateliteDto> ObterPorIdAsync(int id, CancellationToken ct = default);

    /// <summary>Registra a leitura, calcula o risco e gera automaticamente a missão correspondente.</summary>
    Task<LeituraSateliteDto> RegistrarAsync(CreateLeituraSateliteDto dto, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}

public interface IMissaoService
{
    Task<PagedResult<MissaoDto>> ListarAsync(QueryParameters q, int? setorId, StatusMissao? status, CancellationToken ct = default);
    Task<MissaoDto> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<MissaoDto> CriarAsync(CreateMissaoDto dto, CancellationToken ct = default);

    /// <summary>Atualiza o status da missão; ao concluir, credita os pontos ao produtor dono.</summary>
    Task<MissaoDto> AtualizarStatusAsync(int id, AtualizarStatusMissaoDto dto, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}
