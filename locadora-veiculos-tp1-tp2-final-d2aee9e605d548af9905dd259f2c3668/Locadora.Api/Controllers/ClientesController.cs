using Locadora.Api.Data;
using Locadora.Api.Domain;
using Locadora.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController, Route("api/clientes")]
public class ClientesController(LocadoraDbContext db) : ControllerBase
{
    /// <summary>Lista clientes.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Clientes.AsNoTracking().OrderBy(x => x.Id).Select(x => new { x.Id, x.Nome, x.Cpf, x.Email }).ToListAsync());

    /// <summary>Consulta cliente pelo ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var x = await db.Clientes.AsNoTracking().Where(x => x.Id == id).Select(x => new { x.Id, x.Nome, x.Cpf, x.Email }).FirstOrDefaultAsync();
        return x is null ? NotFound() : Ok(x);
    }

    /// <summary>Cria cliente com CPF válido.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar(ClienteInput input)
    {
        if (!CpfValido(input.Cpf)) return BadRequest(new { erro = "CPF inválido." });
        if (await db.Clientes.AnyAsync(x => x.Cpf == input.Cpf || x.Email == input.Email.Trim())) return Conflict(new { erro = "CPF ou e-mail já cadastrado." });
        var x = new Cliente { Nome = input.Nome.Trim(), Cpf = input.Cpf, Email = input.Email.Trim() };
        db.Clientes.Add(x); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obter), new { id = x.Id }, new { x.Id, x.Nome, x.Cpf, x.Email });
    }

    /// <summary>Atualiza cliente.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ClienteInput input)
    {
        var x = await db.Clientes.FindAsync(id);
        if (x is null) return NotFound();
        if (!CpfValido(input.Cpf)) return BadRequest(new { erro = "CPF inválido." });
        if (await db.Clientes.AnyAsync(y => y.Id != id && (y.Cpf == input.Cpf || y.Email == input.Email.Trim()))) return Conflict(new { erro = "CPF ou e-mail já cadastrado." });
        x.Nome = input.Nome.Trim(); x.Cpf = input.Cpf; x.Email = input.Email.Trim();
        await db.SaveChangesAsync(); return NoContent();
    }

    /// <summary>Remove cliente sem aluguéis vinculados.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var x = await db.Clientes.FindAsync(id);
        if (x is null) return NotFound();
        if (await db.Alugueis.AnyAsync(a => a.ClienteId == id)) return Conflict(new { erro = "Cliente possui aluguéis." });
        db.Clientes.Remove(x); await db.SaveChangesAsync(); return NoContent();
    }

    private static bool CpfValido(string cpf)
    {
        if (cpf.Length != 11 || cpf.Any(c => !char.IsDigit(c)) || cpf.All(c => c == cpf[0])) return false;
        for (var tamanho = 9; tamanho <= 10; tamanho++)
        {
            var soma = 0;
            for (var i = 0; i < tamanho; i++) soma += (cpf[i] - '0') * (tamanho + 1 - i);
            var digito = (soma * 10) % 11;
            if (digito == 10) digito = 0;
            if (digito != cpf[tamanho] - '0') return false;
        }
        return true;
    }
}
