using System.ComponentModel.DataAnnotations;
using SetupVault.Api.Models;

namespace SetupVault.Api.DTOs.Perifericos;

/// <summary>Dados para criar ou atualizar um periférico.</summary>
public class PerifericoRequest
{
    /// <example>Hayate Otsu XL</example>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <example>Mousepad de tecido com base de poron, foco em velocidade.</example>
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }

    /// <example>Mousepad</example>
    [Required(ErrorMessage = "O tipo é obrigatório.")]
    [EnumDataType(typeof(TipoPeriferico), ErrorMessage = "Tipo inválido.")]
    public TipoPeriferico? Tipo { get; set; }

    /// <example>389.90</example>
    [Range(0.01, 999999.99, ErrorMessage = "O preço deve estar entre 0,01 e 999.999,99.")]
    public decimal Preco { get; set; }

    /// <example>15</example>
    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
    public int Estoque { get; set; }

    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "Informe um fabricanteId válido.")]
    public int FabricanteId { get; set; }
}
