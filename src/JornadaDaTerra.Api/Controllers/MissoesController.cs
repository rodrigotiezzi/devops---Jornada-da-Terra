using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Services;
using JornadaDaTerra.Api.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace JornadaDaTerra.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MissoesController : ControllerBase
{
    private readonly IMissaoService _service;

    public MissoesController(IMissaoService service) => _service = service;

    /// <summary>Lista as missões (paginado). Filtros opcionais: <c>setorId</c> e <c>status</c>.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MissaoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<MissaoDto>>> Listar(
        [FromQuery] QueryParameters query,
        [FromQuery] int? setorId,
        [FromQuery] StatusMissao? status,
        CancellationToken ct)
        => Ok(await _service.ListarAsync(query, setorId, status, ct));

    /// <summary>Obtém uma missão pelo Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MissaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MissaoDto>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Cria uma missão manualmente (além das geradas automaticamente).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MissaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<MissaoDto>> Criar(
        [FromBody] CreateMissaoDto dto, CancellationToken ct)
    {
        var criada = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    /// <summary>
    /// Atualiza o status da missão (Pendente → EmProgresso → Concluida).
    /// Ao concluir, os pontos da recompensa são creditados ao produtor dono.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(MissaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<MissaoDto>> AtualizarStatus(
        int id, [FromBody] AtualizarStatusMissaoDto dto, CancellationToken ct)
        => Ok(await _service.AtualizarStatusAsync(id, dto, ct));

    /// <summary>Remove uma missão.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
