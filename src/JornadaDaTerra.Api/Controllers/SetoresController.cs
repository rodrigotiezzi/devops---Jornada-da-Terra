using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace JornadaDaTerra.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SetoresController : ControllerBase
{
    private readonly ISetorService _service;

    public SetoresController(ISetorService service) => _service = service;

    /// <summary>Lista os setores (paginado). Use <c>fazendaId</c> para filtrar.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SetorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<SetorDto>>> Listar(
        [FromQuery] QueryParameters query, [FromQuery] int? fazendaId, CancellationToken ct)
        => Ok(await _service.ListarAsync(query, fazendaId, ct));

    /// <summary>Obtém um setor pelo Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SetorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SetorDto>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Cadastra um novo setor em uma fazenda existente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SetorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<SetorDto>> Criar(
        [FromBody] CreateSetorDto dto, CancellationToken ct)
    {
        var criado = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza os dados de um setor.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(SetorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SetorDto>> Atualizar(
        int id, [FromBody] UpdateSetorDto dto, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, dto, ct));

    /// <summary>Remove um setor (e, em cascata, suas missões/leituras).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
