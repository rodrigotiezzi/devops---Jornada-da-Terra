using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace JornadaDaTerra.Api.Controllers;

[ApiController]
[Route("api/leituras-satelite")]
[Produces("application/json")]
public class LeiturasSateliteController : ControllerBase
{
    private readonly ILeituraSateliteService _service;

    public LeiturasSateliteController(ILeituraSateliteService service) => _service = service;

    /// <summary>Lista as leituras de satélite (paginado). Use <c>setorId</c> para filtrar.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LeituraSateliteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<LeituraSateliteDto>>> Listar(
        [FromQuery] QueryParameters query, [FromQuery] int? setorId, CancellationToken ct)
        => Ok(await _service.ListarAsync(query, setorId, ct));

    /// <summary>Obtém uma leitura pelo Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LeituraSateliteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraSateliteDto>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>
    /// Registra uma nova leitura de satélite. O risco é calculado automaticamente e,
    /// quando relevante, uma missão é gerada para o setor (núcleo da gamificação).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(LeituraSateliteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LeituraSateliteDto>> Registrar(
        [FromBody] CreateLeituraSateliteDto dto, CancellationToken ct)
    {
        var criada = await _service.RegistrarAsync(dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    /// <summary>Remove uma leitura de satélite.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
