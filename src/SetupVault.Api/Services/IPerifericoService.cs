using SetupVault.Api.DTOs.Perifericos;
using SetupVault.Api.Models;

namespace SetupVault.Api.Services;

public interface IPerifericoService
{
    Task<IReadOnlyList<PerifericoResponse>> ListarAsync(TipoPeriferico? tipo, int? fabricanteId, CancellationToken ct);
    Task<IReadOnlyList<PerifericoResponse>> ListarPorFabricanteAsync(int fabricanteId, CancellationToken ct);
    Task<PerifericoResponse> ObterPorIdAsync(int id, CancellationToken ct);
    Task<PerifericoResponse> CriarAsync(PerifericoRequest request, CancellationToken ct);
    Task<PerifericoResponse> AtualizarAsync(int id, PerifericoRequest request, CancellationToken ct);
    Task RemoverAsync(int id, CancellationToken ct);
}
