using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace JornadaDaTerra.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FazendasController : ControllerBase
{
    private readonly IFazendaService _service;

    public FazendasController(IFazendaService service) => _service = service;

    /// <summary>Lista as fazendas (paginado). Use <c>produtorId</c> para filtrar por produtor.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<FazendaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FazendaDto>>> Listar(
        [FromQuery] QueryParameters query, [FromQuery] int? produtorId, CancellationToken ct)
        => Ok(await _service.ListarAsync(query, produtorId, ct));

    /// <summary>Obtém uma fazenda pelo Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FazendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FazendaDto>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Cadastra uma nova fazenda para um produtor existente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(FazendaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<FazendaDto>> Criar(
        [FromBody] CreateFazendaDto dto, CancellationToken ct)
    {
        var criada = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    /// <summary>Atualiza os dados de uma fazenda.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(FazendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FazendaDto>> Atualizar(
        int id, [FromBody] UpdateFazendaDto dto, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, dto, ct));

    /// <summary>Remove uma fazenda (e, em cascata, seus setores/missões/leituras).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
