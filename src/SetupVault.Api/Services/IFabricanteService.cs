using SetupVault.Api.DTOs.Fabricantes;

namespace SetupVault.Api.Services;

public interface IFabricanteService
{
    Task<IReadOnlyList<FabricanteResponse>> ListarAsync(CancellationToken ct);
    Task<FabricanteResponse> ObterPorIdAsync(int id, CancellationToken ct);
    Task<FabricanteResponse> CriarAsync(FabricanteRequest request, CancellationToken ct);
    Task<FabricanteResponse> AtualizarAsync(int id, FabricanteRequest request, CancellationToken ct);
    Task RemoverAsync(int id, CancellationToken ct);
}
