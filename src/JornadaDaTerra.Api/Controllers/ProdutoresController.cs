using JornadaDaTerra.Api.Application.Common;
using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace JornadaDaTerra.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutoresController : ControllerBase
{
    private readonly IProdutorService _service;

    public ProdutoresController(IProdutorService service) => _service = service;

    /// <summary>Lista os produtores (paginado).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProdutorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProdutorDto>>> Listar(
        [FromQuery] QueryParameters query, CancellationToken ct)
        => Ok(await _service.ListarAsync(query, ct));

    /// <summary>Obtém um produtor pelo Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutorDto>> ObterPorId(int id, CancellationToken ct)
        => Ok(await _service.ObterPorIdAsync(id, ct));

    /// <summary>Cadastra um novo produtor.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProdutorDto>> Criar(
        [FromBody] CreateProdutorDto dto, CancellationToken ct)
    {
        var criado = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza os dados de um produtor.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProdutorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProdutorDto>> Atualizar(
        int id, [FromBody] UpdateProdutorDto dto, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, dto, ct));

    /// <summary>Remove um produtor (e, em cascata, suas fazendas/setores).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
