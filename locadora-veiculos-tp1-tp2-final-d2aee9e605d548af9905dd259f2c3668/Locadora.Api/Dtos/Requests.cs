using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public record FabricanteInput(
    [Required, StringLength(100, MinimumLength = 2)] string Nome,
    [Required, StringLength(80, MinimumLength = 2)] string Pais);

public record VeiculoInput(
    [Range(1, int.MaxValue)] int FabricanteId,
    [Required, StringLength(100, MinimumLength = 1)] string Modelo,
    [Range(1900, 2100)] int AnoFabricacao,
    [Range(0, int.MaxValue)] int Quilometragem,
    [Required, RegularExpression(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$", ErrorMessage = "Placa deve seguir o padrão brasileiro ABC1234 ou ABC1D23.")] string Placa,
    [Range(typeof(decimal), "0.01", "9999999999.99")] decimal ValorDiaria);

public record ClienteInput(
    [Required, StringLength(120, MinimumLength = 2)] string Nome,
    [Required, RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter 11 dígitos.")] string Cpf,
    [Required, EmailAddress, StringLength(200)] string Email);

public record AluguelInput(
    [Range(1, int.MaxValue)] int ClienteId,
    [Range(1, int.MaxValue)] int VeiculoId,
    DateTime Inicio,
    DateTime FimPrevisto);

public record DevolucaoInput(DateTime Devolucao, [Range(0, int.MaxValue)] int QuilometragemFinal);

public record PagamentoInput(
    [Range(1, int.MaxValue)] int AluguelId,
    DateTime Data,
    [Range(typeof(decimal), "0.01", "9999999999.99")] decimal Valor,
    [Required, RegularExpression("^(Pix|Cartao|Dinheiro|Transferencia)$", ErrorMessage = "Forma: Pix, Cartao, Dinheiro ou Transferencia.")] string Forma);
