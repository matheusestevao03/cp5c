namespace SetupVault.Api.Models;

/// <summary>Empresa que fabrica periféricos (ex.: Logitech, Artisan, Razer).</summary>
public class Fabricante
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string PaisOrigem { get; set; } = string.Empty;
    public string? SiteOficial { get; set; }
    public DateTime DataCadastro { get; set; }

    public ICollection<Periferico> Perifericos { get; set; } = new List<Periferico>();
}
