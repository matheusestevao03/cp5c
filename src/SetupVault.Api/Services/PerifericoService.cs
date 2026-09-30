using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SetupVault.Api.Data;
using SetupVault.Api.DTOs.Perifericos;
using SetupVault.Api.Exceptions;
using SetupVault.Api.Models;

namespace SetupVault.Api.Services;

public class PerifericoService(AppDbContext context) : IPerifericoService
{
    private static readonly Expression<Func<Periferico, PerifericoResponse>> Projecao = p => new PerifericoResponse
    {
        Id = p.Id,
        Nome = p.Nome,
        Descricao = p.Descricao,
        Tipo = p.Tipo,
        Preco = p.Preco,
        Estoque = p.Estoque,
        FabricanteId = p.FabricanteId,
        FabricanteNome = p.Fabricante.Nome,
        DataCadastro = p.DataCadastro,
        DataAtualizacao = p.DataAtualizacao
    };

    public async Task<IReadOnlyList<PerifericoResponse>> ListarAsync(
        TipoPeriferico? tipo, int? fabricanteId, CancellationToken ct)
    {
        var query = context.Perifericos.AsNoTracking();

        if (tipo.HasValue)
            query = query.Where(p => p.Tipo == tipo.Value);

        if (fabricanteId.HasValue)
            query = query.Where(p => p.FabricanteId == fabricanteId.Value);

        return await query
            .OrderBy(p => p.Nome)
            .Select(Projecao)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PerifericoResponse>> ListarPorFabricanteAsync(int fabricanteId, CancellationToken ct)
    {
        var fabricanteExiste = await context.Fabricantes.AnyAsync(f => f.Id == fabricanteId, ct);
        if (!fabricanteExiste)
            throw new NotFoundException($"Fabricante com id {fabricanteId} não encontrado.");

        return await ListarAsync(tipo: null, fabricanteId, ct);
    }

    public async Task<PerifericoResponse> ObterPorIdAsync(int id, CancellationToken ct)
    {
        return await context.Perifericos
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(Projecao)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException($"Periférico com id {id} não encontrado.");
    }

    public async Task<PerifericoResponse> CriarAsync(PerifericoRequest request, CancellationToken ct)
    {
        var fabricante = await ObterFabricanteOuFalharAsync(request.FabricanteId, ct);

        var periferico = new Periferico
        {
            Nome = request.Nome.Trim(),
            Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim(),
            Tipo = request.Tipo!.Value,
            Preco = request.Preco,
            Estoque = request.Estoque,
            FabricanteId = fabricante.Id,
            DataCadastro = DateTime.UtcNow
        };

        context.Perifericos.Add(periferico);
        await context.SaveChangesAsync(ct);

        return ParaResponse(periferico, fabricante.Nome);
    }

    public async Task<PerifericoResponse> AtualizarAsync(int id, PerifericoRequest request, CancellationToken ct)
    {
        var periferico = await context.Perifericos.FindAsync([id], ct)
            ?? throw new NotFoundException($"Periférico com id {id} não encontrado.");

        var fabricante = await ObterFabricanteOuFalharAsync(request.FabricanteId, ct);

        periferico.Nome = request.Nome.Trim();
        periferico.Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim();
        periferico.Tipo = request.Tipo!.Value;
        periferico.Preco = request.Preco;
        periferico.Estoque = request.Estoque;
        periferico.FabricanteId = fabricante.Id;
        periferico.DataAtualizacao = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return ParaResponse(periferico, fabricante.Nome);
    }

    public async Task RemoverAsync(int id, CancellationToken ct)
    {
        var periferico = await context.Perifericos.FindAsync([id], ct)
            ?? throw new NotFoundException($"Periférico com id {id} não encontrado.");

        context.Perifericos.Remove(periferico);
        await context.SaveChangesAsync(ct);
    }

    // FabricanteId inválido no corpo é erro do cliente (400), não "recurso da rota não encontrado" (404)
    private async Task<Fabricante> ObterFabricanteOuFalharAsync(int fabricanteId, CancellationToken ct)
    {
        return await context.Fabricantes.AsNoTracking().FirstOrDefaultAsync(f => f.Id == fabricanteId, ct)
            ?? throw new BusinessRuleException($"O fabricante informado (id {fabricanteId}) não existe.");
    }

    private static PerifericoResponse ParaResponse(Periferico p, string fabricanteNome) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Descricao = p.Descricao,
        Tipo = p.Tipo,
        Preco = p.Preco,
        Estoque = p.Estoque,
        FabricanteId = p.FabricanteId,
        FabricanteNome = fabricanteNome,
        DataCadastro = p.DataCadastro,
        DataAtualizacao = p.DataAtualizacao
    };
}
