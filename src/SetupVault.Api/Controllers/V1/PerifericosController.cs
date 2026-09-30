using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SetupVault.Api.DTOs.Perifericos;
using SetupVault.Api.Models;
using SetupVault.Api.Services;

namespace SetupVault.Api.Controllers.V1;

/// <summary>Gerenciamento de periféricos (mouses, mousepads, teclados etc.).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/perifericos")]
[Produces("application/json")]
public class PerifericosController(IPerifericoService perifericoService) : ControllerBase
{
    /// <summary>Lista periféricos, com filtros opcionais por tipo e fabricante.</summary>
    /// <param name="tipo">Ex.: Mouse, Mousepad, Teclado, Headset, Monitor, Webcam, Microfone, Outro</param>
    /// <param name="fabricanteId">Id do fabricante</param>
    /// <param name="ct">Token de cancelamento</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PerifericoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PerifericoResponse>>> Listar(
        [FromQuery] TipoPeriferico? tipo,
        [FromQuery] int? fabricanteId,
        CancellationToken ct)
    {
        return Ok(await perifericoService.ListarAsync(tipo, fabricanteId, ct));
    }

    /// <summary>Busca um periférico pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PerifericoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PerifericoResponse>> ObterPorId(int id, CancellationToken ct)
    {
        return Ok(await perifericoService.ObterPorIdAsync(id, ct));
    }

    /// <summary>Cadastra um novo periférico.</summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(PerifericoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PerifericoResponse>> Criar([FromBody] PerifericoRequest request, CancellationToken ct)
    {
        var criado = await perifericoService.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id, version = RouteData.Values["version"] }, criado);
    }

    /// <summary>Atualiza todos os dados de um periférico.</summary>
    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(PerifericoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PerifericoResponse>> Atualizar(int id, [FromBody] PerifericoRequest request, CancellationToken ct)
    {
        return Ok(await perifericoService.AtualizarAsync(id, request, ct));
    }

    /// <summary>Remove um periférico.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await perifericoService.RemoverAsync(id, ct);
        return NoContent();
    }
}
