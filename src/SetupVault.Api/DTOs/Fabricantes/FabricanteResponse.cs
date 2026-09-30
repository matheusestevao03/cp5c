namespace SetupVault.Api.DTOs.Fabricantes;

public class FabricanteResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string PaisOrigem { get; init; } = string.Empty;
    public string? SiteOficial { get; init; }
    public DateTime DataCadastro { get; init; }
    public int QuantidadePerifericos { get; init; }
}
