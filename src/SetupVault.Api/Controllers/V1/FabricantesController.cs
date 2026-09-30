using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SetupVault.Api.DTOs.Fabricantes;
using SetupVault.Api.DTOs.Perifericos;
using SetupVault.Api.Services;

namespace SetupVault.Api.Controllers.V1;

/// <summary>Gerenciamento de fabricantes de periféricos.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/fabricantes")]
[Produces("application/json")]
public class FabricantesController(
    IFabricanteService fabricanteService,
    IPerifericoService perifericoService) : ControllerBase
{
    /// <summary>Lista todos os fabricantes, em ordem alfabética.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FabricanteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FabricanteResponse>>> Listar(CancellationToken ct)
    {
        return Ok(await fabricanteService.ListarAsync(ct));
    }

    /// <summary>Busca um fabricante pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FabricanteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FabricanteResponse>> ObterPorId(int id, CancellationToken ct)
    {
        return Ok(await fabricanteService.ObterPorIdAsync(id, ct));
    }

    /// <summary>Lista os periféricos de um fabricante.</summary>
    [HttpGet("{id:int}/perifericos")]
    [ProducesResponseType(typeof(IEnumerable<PerifericoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PerifericoResponse>>> ListarPerifericos(int id, CancellationToken ct)
    {
        return Ok(await perifericoService.ListarPorFabricanteAsync(id, ct));
    }

    /// <summary>Cadastra um novo fabricante.</summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(FabricanteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FabricanteResponse>> Criar([FromBody] FabricanteRequest request, CancellationToken ct)
    {
        var criado = await fabricanteService.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id, version = RouteData.Values["version"] }, criado);
    }

    /// <summary>Atualiza todos os dados de um fabricante.</summary>
    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(FabricanteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FabricanteResponse>> Atualizar(int id, [FromBody] FabricanteRequest request, CancellationToken ct)
    {
        return Ok(await fabricanteService.AtualizarAsync(id, request, ct));
    }

    /// <summary>Remove um fabricante (somente se não tiver periféricos).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await fabricanteService.RemoverAsync(id, ct);
        return NoContent();
    }
}
