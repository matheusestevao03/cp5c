using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SetupVault.Api.Data;
using SetupVault.Api.DTOs.Fabricantes;
using SetupVault.Api.Exceptions;
using SetupVault.Api.Models;

namespace SetupVault.Api.Services;

public class FabricanteService(AppDbContext context) : IFabricanteService
{
    // Projeção traduzida para SQL pelo EF Core (evita carregar a entidade inteira)
    private static readonly Expression<Func<Fabricante, FabricanteResponse>> Projecao = f => new FabricanteResponse
    {
        Id = f.Id,
        Nome = f.Nome,
        PaisOrigem = f.PaisOrigem,
        SiteOficial = f.SiteOficial,
        DataCadastro = f.DataCadastro,
        QuantidadePerifericos = f.Perifericos.Count
    };

    public async Task<IReadOnlyList<FabricanteResponse>> ListarAsync(CancellationToken ct)
    {
        return await context.Fabricantes
            .AsNoTracking()
            .OrderBy(f => f.Nome)
            .Select(Projecao)
            .ToListAsync(ct);
    }

    public async Task<FabricanteResponse> ObterPorIdAsync(int id, CancellationToken ct)
    {
        return await context.Fabricantes
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(Projecao)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"Fabricante com id {id} não encontrado.");
    }

    public async Task<FabricanteResponse> CriarAsync(FabricanteRequest request, CancellationToken ct)
    {
        var nome = request.Nome.Trim();
        await GarantirNomeUnicoAsync(nome, idIgnorado: null, ct);

        var fabricante = new Fabricante
        {
            Nome = nome,
            PaisOrigem = request.PaisOrigem.Trim(),
            SiteOficial = string.IsNullOrWhiteSpace(request.SiteOficial) ? null : request.SiteOficial.Trim(),
            DataCadastro = DateTime.UtcNow
        };

        context.Fabricantes.Add(fabricante);
        await context.SaveChangesAsync(ct);

        return ParaResponse(fabricante, quantidadePerifericos: 0);
    }

    public async Task<FabricanteResponse> AtualizarAsync(int id, FabricanteRequest request, CancellationToken ct)
    {
        var fabricante = await context.Fabricantes.FindAsync([id], ct)
            ?? throw new NotFoundException($"Fabricante com id {id} não encontrado.");

        var nome = request.Nome.Trim();
        await GarantirNomeUnicoAsync(nome, idIgnorado: id, ct);

        fabricante.Nome = nome;
        fabricante.PaisOrigem = request.PaisOrigem.Trim();
        fabricante.SiteOficial = string.IsNullOrWhiteSpace(request.SiteOficial) ? null : request.SiteOficial.Trim();

        await context.SaveChangesAsync(ct);

        var quantidade = await context.Perifericos.CountAsync(p => p.FabricanteId == id, ct);
        return ParaResponse(fabricante, quantidade);
    }

    public async Task RemoverAsync(int id, CancellationToken ct)
    {
        var fabricante = await context.Fabricantes.FindAsync([id], ct)
            ?? throw new NotFoundException($"Fabricante com id {id} não encontrado.");

        var possuiPerifericos = await context.Perifericos.AnyAsync(p => p.FabricanteId == id, ct);
        if (possuiPerifericos)
            throw new ConflictException(
                "Não é possível excluir um fabricante que possui periféricos cadastrados. Remova os periféricos primeiro.");

        context.Fabricantes.Remove(fabricante);
        await context.SaveChangesAsync(ct);
    }

    private async Task GarantirNomeUnicoAsync(string nome, int? idIgnorado, CancellationToken ct)
    {
        var nomeNormalizado = nome.ToLower();
        var existe = await context.Fabricantes.AnyAsync(
            f => f.Nome.ToLower() == nomeNormalizado && (idIgnorado == null || f.Id != idIgnorado), ct);

        if (existe)
            throw new ConflictException($"Já existe um fabricante com o nome '{nome}'.");
    }

    private static FabricanteResponse ParaResponse(Fabricante f, int quantidadePerifericos) => new()
    {
        Id = f.Id,
        Nome = f.Nome,
        PaisOrigem = f.PaisOrigem,
        SiteOficial = f.SiteOficial,
        DataCadastro = f.DataCadastro,
        QuantidadePerifericos = quantidadePerifericos
    };
}
