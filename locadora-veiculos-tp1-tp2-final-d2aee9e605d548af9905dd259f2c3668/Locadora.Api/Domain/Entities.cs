namespace Locadora.Api.Domain;

public class Fabricante
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Pais { get; set; } = "";
    public List<Veiculo> Veiculos { get; set; } = [];
}

public class Veiculo
{
    public int Id { get; set; }
    public int FabricanteId { get; set; }
    public Fabricante Fabricante { get; set; } = null!;
    public string Modelo { get; set; } = "";
    public int AnoFabricacao { get; set; }
    public int Quilometragem { get; set; }
    public string Placa { get; set; } = "";
    public decimal ValorDiaria { get; set; }
    public List<Aluguel> Alugueis { get; set; } = [];
}

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Cpf { get; set; } = "";
    public string Email { get; set; } = "";
    public List<Aluguel> Alugueis { get; set; } = [];
}

public class Aluguel
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public int VeiculoId { get; set; }
    public Veiculo Veiculo { get; set; } = null!;
    public DateTime Inicio { get; set; }
    public DateTime FimPrevisto { get; set; }
    public DateTime? Devolucao { get; set; }
    public int QuilometragemInicial { get; set; }
    public int? QuilometragemFinal { get; set; }
    public decimal ValorDiaria { get; set; }
    public decimal? ValorTotal { get; set; }
    public List<Pagamento> Pagamentos { get; set; } = [];
}

public class Pagamento
{
    public int Id { get; set; }
    public int AluguelId { get; set; }
    public Aluguel Aluguel { get; set; } = null!;
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string Forma { get; set; } = "";
}
