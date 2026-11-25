using Locadora.Api.Data;
using Locadora.Api.Domain;
using Locadora.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController, Route("api/veiculos")]
public class VeiculosController(LocadoraDbContext db) : ControllerBase
{
    /// <summary>Lista veículos e seus fabricantes.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Veiculos.AsNoTracking().OrderBy(x => x.Id)
        .Select(x => new { x.Id, x.FabricanteId, Fabricante = x.Fabricante.Nome, x.Modelo, x.AnoFabricacao, x.Quilometragem, x.Placa, x.ValorDiaria }).ToListAsync());

    /// <summary>Consulta veículo pelo ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var x = await db.Veiculos.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new { x.Id, x.FabricanteId, Fabricante = x.Fabricante.Nome, x.Modelo, x.AnoFabricacao, x.Quilometragem, x.Placa, x.ValorDiaria }).FirstOrDefaultAsync();
        return x is null ? NotFound() : Ok(x);
    }

    /// <summary>Cria veículo.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar(VeiculoInput input)
    {
        if (!await db.Fabricantes.AnyAsync(x => x.Id == input.FabricanteId)) return BadRequest(new { erro = "Fabricante inexistente." });
        if (await db.Veiculos.AnyAsync(x => x.Placa == input.Placa.ToUpperInvariant())) return Conflict(new { erro = "Placa já cadastrada." });
        var x = new Veiculo { FabricanteId = input.FabricanteId, Modelo = input.Modelo.Trim(), AnoFabricacao = input.AnoFabricacao, Quilometragem = input.Quilometragem, Placa = input.Placa.ToUpperInvariant(), ValorDiaria = input.ValorDiaria };
        db.Veiculos.Add(x); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obter), new { id = x.Id }, new { x.Id, x.FabricanteId, x.Modelo, x.AnoFabricacao, x.Quilometragem, x.Placa, x.ValorDiaria });
    }

    /// <summary>Atualiza veículo. A quilometragem não pode diminuir.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, VeiculoInput input)
    {
        var x = await db.Veiculos.FindAsync(id);
        if (x is null) return NotFound();
        if (!await db.Fabricantes.AnyAsync(f => f.Id == input.FabricanteId)) return BadRequest(new { erro = "Fabricante inexistente." });
        if (input.Quilometragem < x.Quilometragem) return BadRequest(new { erro = "Quilometragem não pode diminuir." });
        if (await db.Alugueis.AnyAsync(a => a.VeiculoId == id && a.Devolucao == null) && (input.Quilometragem != x.Quilometragem || input.FabricanteId != x.FabricanteId || input.Placa.ToUpperInvariant() != x.Placa))
            return Conflict(new { erro = "Veículo em aluguel aberto: não altere placa, fabricante ou quilometragem." });
        if (await db.Veiculos.AnyAsync(v => v.Id != id && v.Placa == input.Placa.ToUpperInvariant())) return Conflict(new { erro = "Placa já cadastrada." });
        x.FabricanteId = input.FabricanteId; x.Modelo = input.Modelo.Trim(); x.AnoFabricacao = input.AnoFabricacao;
        x.Quilometragem = input.Quilometragem; x.Placa = input.Placa.ToUpperInvariant(); x.ValorDiaria = input.ValorDiaria;
        await db.SaveChangesAsync(); return NoContent();
    }

    /// <summary>Remove veículo sem histórico de aluguel.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var x = await db.Veiculos.FindAsync(id);
        if (x is null) return NotFound();
        if (await db.Alugueis.AnyAsync(a => a.VeiculoId == id)) return Conflict(new { erro = "Veículo possui aluguéis." });
        db.Veiculos.Remove(x); await db.SaveChangesAsync(); return NoContent();
    }
}
