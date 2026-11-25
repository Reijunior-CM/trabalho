using System.Data;
using Locadora.Api.Data;
using Locadora.Api.Domain;
using Locadora.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController, Route("api/alugueis")]
public class AlugueisController(LocadoraDbContext db) : ControllerBase
{
    /// <summary>Lista aluguéis com cliente e veículo.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Alugueis.AsNoTracking().OrderBy(x => x.Id).Select(x => new
    {
        x.Id, x.ClienteId, Cliente = x.Cliente.Nome, x.VeiculoId, Veiculo = x.Veiculo.Modelo,
        x.Inicio, x.FimPrevisto, x.Devolucao, x.QuilometragemInicial, x.QuilometragemFinal, x.ValorDiaria, x.ValorTotal
    }).ToListAsync());

    /// <summary>Consulta aluguel pelo ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var x = await db.Alugueis.AsNoTracking().Where(x => x.Id == id).Select(x => new
        {
            x.Id, x.ClienteId, Cliente = x.Cliente.Nome, x.VeiculoId, Veiculo = x.Veiculo.Modelo,
            x.Inicio, x.FimPrevisto, x.Devolucao, x.QuilometragemInicial, x.QuilometragemFinal, x.ValorDiaria, x.ValorTotal
        }).FirstOrDefaultAsync();
        return x is null ? NotFound() : Ok(x);
    }

    /// <summary>Abre aluguel e registra a quilometragem inicial e a diária vigente.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar(AluguelInput input)
    {
        if (input.Inicio == default || input.FimPrevisto <= input.Inicio) return BadRequest(new { erro = "Período inválido." });
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        if (!await db.Clientes.AnyAsync(c => c.Id == input.ClienteId)) return BadRequest(new { erro = "Cliente inexistente." });
        var veiculo = await db.Veiculos.FindAsync(input.VeiculoId);
        if (veiculo is null) return BadRequest(new { erro = "Veículo inexistente." });
        if (await db.Alugueis.AnyAsync(a => a.VeiculoId == input.VeiculoId && a.Inicio < input.FimPrevisto && (a.Devolucao == null || a.Devolucao > input.Inicio)))
            return Conflict(new { erro = "Veículo indisponível no período." });
        var x = new Aluguel { ClienteId = input.ClienteId, VeiculoId = input.VeiculoId, Inicio = input.Inicio, FimPrevisto = input.FimPrevisto, QuilometragemInicial = veiculo.Quilometragem, ValorDiaria = veiculo.ValorDiaria };
        db.Alugueis.Add(x); await db.SaveChangesAsync(); await tx.CommitAsync();
        return CreatedAtAction(nameof(Obter), new { id = x.Id }, new { x.Id, x.ClienteId, x.VeiculoId, x.Inicio, x.FimPrevisto, x.QuilometragemInicial, x.ValorDiaria });
    }

    /// <summary>Atualiza o período planejado ou cliente de aluguel aberto.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, AluguelInput input)
    {
        if (input.Inicio == default || input.FimPrevisto <= input.Inicio) return BadRequest(new { erro = "Período inválido." });
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var x = await db.Alugueis.FindAsync(id);
        if (x is null) return NotFound();
        if (x.Devolucao is not null) return Conflict(new { erro = "Aluguel devolvido é histórico e não pode ser alterado." });
        if (x.VeiculoId != input.VeiculoId) return BadRequest(new { erro = "Veículo do aluguel não pode ser trocado." });
        if (!await db.Clientes.AnyAsync(c => c.Id == input.ClienteId)) return BadRequest(new { erro = "Cliente inexistente." });
        if (await db.Alugueis.AnyAsync(a => a.Id != id && a.VeiculoId == input.VeiculoId && a.Inicio < input.FimPrevisto && (a.Devolucao == null || a.Devolucao > input.Inicio)))
            return Conflict(new { erro = "Veículo indisponível no período." });
        x.ClienteId = input.ClienteId; x.Inicio = input.Inicio; x.FimPrevisto = input.FimPrevisto;
        await db.SaveChangesAsync(); await tx.CommitAsync(); return NoContent();
    }

    /// <summary>Registra a devolução, atualiza a quilometragem e calcula as diárias por teto de 24 horas.</summary>
    [HttpPut("{id:int}/devolucao")]
    public async Task<IActionResult> Devolver(int id, DevolucaoInput input)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var x = await db.Alugueis.Include(a => a.Veiculo).FirstOrDefaultAsync(a => a.Id == id);
        if (x is null) return NotFound();
        if (x.Devolucao is not null) return Conflict(new { erro = "Aluguel já devolvido." });
        if (input.Devolucao < x.Inicio || input.QuilometragemFinal < x.QuilometragemInicial || input.QuilometragemFinal < x.Veiculo.Quilometragem)
            return BadRequest(new { erro = "Data ou quilometragem de devolução inválida." });
        x.Devolucao = input.Devolucao;
        x.QuilometragemFinal = input.QuilometragemFinal;
        x.ValorTotal = Math.Max(1, (decimal)Math.Ceiling((input.Devolucao - x.Inicio).TotalDays)) * x.ValorDiaria;
        x.Veiculo.Quilometragem = input.QuilometragemFinal;
        await db.SaveChangesAsync(); await tx.CommitAsync();
        return Ok(new { x.Id, x.Devolucao, x.QuilometragemInicial, x.QuilometragemFinal, x.ValorDiaria, x.ValorTotal });
    }

    /// <summary>Remove aluguel aberto sem pagamentos. Aluguéis devolvidos preservam o histórico.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var x = await db.Alugueis.FindAsync(id);
        if (x is null) return NotFound();
        if (x.Devolucao is not null || await db.Pagamentos.AnyAsync(p => p.AluguelId == id)) return Conflict(new { erro = "Aluguel com devolução ou pagamento não pode ser excluído." });
        db.Alugueis.Remove(x); await db.SaveChangesAsync(); return NoContent();
    }
}
