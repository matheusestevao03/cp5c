namespace SetupVault.Api.Models;

/// <summary>Item do setup (mouse, mousepad, teclado etc.) com preço e estoque.</summary>
public class Periferico
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoPeriferico Tipo { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    public int FabricanteId { get; set; }
    public Fabricante Fabricante { get; set; } = null!;
}
