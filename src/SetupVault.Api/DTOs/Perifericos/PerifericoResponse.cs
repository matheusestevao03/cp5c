using SetupVault.Api.Models;

namespace SetupVault.Api.DTOs.Perifericos;

public class PerifericoResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public TipoPeriferico Tipo { get; init; }
    public decimal Preco { get; init; }
    public int Estoque { get; init; }
    public int FabricanteId { get; init; }
    public string FabricanteNome { get; init; } = string.Empty;
    public DateTime DataCadastro { get; init; }
    public DateTime? DataAtualizacao { get; init; }
}
