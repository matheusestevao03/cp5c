using System.ComponentModel.DataAnnotations;

namespace SetupVault.Api.DTOs.Fabricantes;

/// <summary>Dados para criar ou atualizar um fabricante.</summary>
public class FabricanteRequest
{
    /// <example>Artisan</example>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <example>Japão</example>
    [Required(ErrorMessage = "O país de origem é obrigatório.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "O país deve ter entre 2 e 60 caracteres.")]
    public string PaisOrigem { get; set; } = string.Empty;

    /// <example>https://www.artisan-jp.com</example>
    [Url(ErrorMessage = "Informe uma URL válida (ex.: https://site.com).")]
    [StringLength(200, ErrorMessage = "A URL deve ter no máximo 200 caracteres.")]
    public string? SiteOficial { get; set; }
}
