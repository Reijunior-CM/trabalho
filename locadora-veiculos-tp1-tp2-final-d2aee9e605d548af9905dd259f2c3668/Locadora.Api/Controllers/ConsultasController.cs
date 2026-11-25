using Locadora.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController, Route("api/consultas")]
public class ConsultasController(LocadoraDbContext db) : ControllerBase
{
    /// <summary>Filtro 1: veículos de um fabricante. INNER JOIN Veiculos/Fabricantes.</summary>
    [HttpGet("veiculos-por-fabricante")]
    public async Task<IActionResult> VeiculosPorFabricante([FromQuery] int fabricanteId)
    {
        if (fabricanteId <= 0) return BadRequest(new { erro = "fabricanteId deve ser positivo." });
        var dados = await (from v in db.Veiculos join f in db.Fabricantes on v.FabricanteId equals f.Id
            where f.Id == fabricanteId select new { v.Id, v.Modelo, v.Placa, Fabricante = f.Nome }).ToListAsync();
        return Ok(dados);
    }

    /// <summary>Filtro 2: aluguéis de um cliente. INNER JOIN Alugueis/Clientes/Veiculos.</summary>
    [HttpGet("alugueis-por-cliente")]
    public async Task<IActionResult> AlugueisPorCliente([FromQuery] int clienteId)
    {
        if (clienteId <= 0) return BadRequest(new { erro = "clienteId deve ser positivo." });
        var dados = await (from a in db.Alugueis join c in db.Clientes on a.ClienteId equals c.Id
            join v in db.Veiculos on a.VeiculoId equals v.Id where c.Id == clienteId
            select new { a.Id, Cliente = c.Nome, Veiculo = v.Modelo, v.Placa, a.Inicio, a.FimPrevisto, a.Devolucao }).ToListAsync();
        return Ok(dados);
    }

    /// <summary>Filtro 3: aluguéis abertos em uma data. INNER JOIN Alugueis/Clientes/Veiculos.</summary>
    [HttpGet("alugueis-abertos")]
    public async Task<IActionResult> AlugueisAbertos([FromQuery] DateTime em)
    {
        if (em == default) return BadRequest(new { erro = "Informe em (data e hora)." });
        var dados = await (from a in db.Alugueis join c in db.Clientes on a.ClienteId equals c.Id
            join v in db.Veiculos on a.VeiculoId equals v.Id
            where a.Inicio <= em && (a.Devolucao == null || a.Devolucao > em)
            select new { a.Id, Cliente = c.Nome, Veiculo = v.Modelo, v.Placa, a.Inicio, a.FimPrevisto }).ToListAsync();
        return Ok(dados);
    }

    /// <summary>Filtro 4: pagamentos do cliente. INNER JOIN Pagamentos/Alugueis/Clientes.</summary>
    [HttpGet("pagamentos-por-cliente")]
    public async Task<IActionResult> PagamentosPorCliente([FromQuery] int clienteId)
    {
        if (clienteId <= 0) return BadRequest(new { erro = "clienteId deve ser positivo." });
        var dados = await (from p in db.Pagamentos join a in db.Alugueis on p.AluguelId equals a.Id
            join c in db.Clientes on a.ClienteId equals c.Id where c.Id == clienteId
            select new { p.Id, p.AluguelId, Cliente = c.Nome, p.Data, p.Valor, p.Forma }).ToListAsync();
        return Ok(dados);
    }

    /// <summary>Filtro 5: veículos sem histórico de aluguel. LEFT JOIN Veiculos/Alugueis.</summary>
    [HttpGet("veiculos-sem-aluguel")]
    public async Task<IActionResult> VeiculosSemAluguel([FromQuery] int? fabricanteId)
    {
        if (fabricanteId is <= 0) return BadRequest(new { erro = "fabricanteId deve ser positivo." });
        var dados = await (from v in db.Veiculos join a in db.Alugueis on v.Id equals a.VeiculoId into historico
            from a in historico.DefaultIfEmpty()
            where a == null && (fabricanteId == null || v.FabricanteId == fabricanteId)
            select new { v.Id, v.Modelo, v.Placa, v.FabricanteId }).ToListAsync();
        return Ok(dados);
    }
}
