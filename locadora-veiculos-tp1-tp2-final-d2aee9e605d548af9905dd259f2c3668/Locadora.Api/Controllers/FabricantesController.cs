using Locadora.Api.Data;
using Locadora.Api.Domain;
using Locadora.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController, Route("api/fabricantes")]
public class FabricantesController(LocadoraDbContext db) : ControllerBase
{
    /// <summary>Lista fabricantes.</summary>
    [HttpGet, ProducesResponseType(typeof(List<Fabricante>), 200)]
    public async Task<IActionResult> Listar() => Ok(await db.Fabricantes.AsNoTracking().OrderBy(x => x.Id).Select(x => new { x.Id, x.Nome, x.Pais }).ToListAsync());

    /// <summary>Consulta fabricante pelo ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var x = await db.Fabricantes.AsNoTracking().Where(x => x.Id == id).Select(x => new { x.Id, x.Nome, x.Pais }).FirstOrDefaultAsync();
        return x is null ? NotFound() : Ok(x);
    }

    /// <summary>Cria fabricante.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar(FabricanteInput input)
    {
        if (await db.Fabricantes.AnyAsync(x => x.Nome == input.Nome.Trim())) return Conflict(new { erro = "Nome já cadastrado." });
        var x = new Fabricante { Nome = input.Nome.Trim(), Pais = input.Pais.Trim() };
        db.Fabricantes.Add(x); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obter), new { id = x.Id }, new { x.Id, x.Nome, x.Pais });
    }

    /// <summary>Atualiza fabricante.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, FabricanteInput input)
    {
        var x = await db.Fabricantes.FindAsync(id);
        if (x is null) return NotFound();
        if (await db.Fabricantes.AnyAsync(y => y.Id != id && y.Nome == input.Nome.Trim())) return Conflict(new { erro = "Nome já cadastrado." });
        x.Nome = input.Nome.Trim(); x.Pais = input.Pais.Trim();
        await db.SaveChangesAsync(); return NoContent();
    }

    /// <summary>Remove fabricante sem veículos vinculados.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var x = await db.Fabricantes.FindAsync(id);
        if (x is null) return NotFound();
        if (await db.Veiculos.AnyAsync(v => v.FabricanteId == id)) return Conflict(new { erro = "Fabricante possui veículos." });
        db.Fabricantes.Remove(x); await db.SaveChangesAsync(); return NoContent();
    }
}
