using System.Data;
using Locadora.Api.Data;
using Locadora.Api.Domain;
using Locadora.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController, Route("api/pagamentos")]
public class PagamentosController(LocadoraDbContext db) : ControllerBase
{
    /// <summary>Lista pagamentos.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Pagamentos.AsNoTracking().OrderBy(x => x.Id)
        .Select(x => new { x.Id, x.AluguelId, x.Data, x.Valor, x.Forma }).ToListAsync());

    /// <summary>Consulta pagamento pelo ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var x = await db.Pagamentos.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new { x.Id, x.AluguelId, x.Data, x.Valor, x.Forma }).FirstOrDefaultAsync();
        return x is null ? NotFound() : Ok(x);
    }

    /// <summary>Registra pagamento de aluguel devolvido sem ultrapassar o total.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar(PagamentoInput input)
    {
        if (input.Data == default) return BadRequest(new { erro = "Data obrigatória." });
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var aluguel = await db.Alugueis.FindAsync(input.AluguelId);
        if (aluguel is null) return BadRequest(new { erro = "Aluguel inexistente." });
        if (aluguel.ValorTotal is null || aluguel.Devolucao is null) return Conflict(new { erro = "Devolva o veículo antes de registrar pagamento." });
        if (input.Data < aluguel.Inicio) return BadRequest(new { erro = "Pagamento anterior ao aluguel." });
        var pago = await db.Pagamentos.Where(p => p.AluguelId == input.AluguelId).SumAsync(p => (decimal?)p.Valor) ?? 0;
        if (pago + input.Valor > aluguel.ValorTotal) return Conflict(new { erro = "Pagamento excede o valor total do aluguel." });
        var x = new Pagamento { AluguelId = input.AluguelId, Data = input.Data, Valor = input.Valor, Forma = input.Forma };
        db.Pagamentos.Add(x); await db.SaveChangesAsync(); await tx.CommitAsync();
        return CreatedAtAction(nameof(Obter), new { id = x.Id }, new { x.Id, x.AluguelId, x.Data, x.Valor, x.Forma });
    }

    /// <summary>Atualiza pagamento dentro do saldo do aluguel.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, PagamentoInput input)
    {
        if (input.Data == default) return BadRequest(new { erro = "Data obrigatória." });
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var x = await db.Pagamentos.FindAsync(id);
        if (x is null) return NotFound();
        if (x.AluguelId != input.AluguelId) return BadRequest(new { erro = "Aluguel do pagamento não pode ser alterado." });
        var aluguel = await db.Alugueis.FindAsync(x.AluguelId);
        if (aluguel is null || aluguel.ValorTotal is null) return Conflict(new { erro = "Aluguel não devolvido." });
        if (input.Data < aluguel.Inicio) return BadRequest(new { erro = "Pagamento anterior ao aluguel." });
        var pago = await db.Pagamentos.Where(p => p.AluguelId == x.AluguelId && p.Id != id).SumAsync(p => (decimal?)p.Valor) ?? 0;
        if (pago + input.Valor > aluguel.ValorTotal) return Conflict(new { erro = "Pagamento excede o valor total do aluguel." });
        x.Data = input.Data; x.Valor = input.Valor; x.Forma = input.Forma;
        await db.SaveChangesAsync(); await tx.CommitAsync(); return NoContent();
    }

    /// <summary>Remove pagamento.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var x = await db.Pagamentos.FindAsync(id);
        if (x is null) return NotFound();
        db.Pagamentos.Remove(x); await db.SaveChangesAsync(); return NoContent();
    }
}
