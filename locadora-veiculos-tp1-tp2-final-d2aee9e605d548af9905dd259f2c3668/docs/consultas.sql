-- Equivalentes SQL dos cinco filtros REST. Ajuste os IDs e datas à sua base.
USE LocadoraDb;
GO
DECLARE @FabricanteId int = 1, @ClienteId int = 1, @Em datetime2 = '2026-12-02T12:00:00';

-- 1. INNER JOIN: veículos por fabricante
SELECT v.Id, v.Modelo, v.Placa, f.Nome AS Fabricante
FROM Veiculos v INNER JOIN Fabricantes f ON f.Id = v.FabricanteId
WHERE f.Id = @FabricanteId;

-- 2. INNER JOIN: aluguéis por cliente
SELECT a.Id, c.Nome AS Cliente, v.Modelo AS Veiculo, v.Placa, a.Inicio, a.FimPrevisto, a.Devolucao
FROM Alugueis a INNER JOIN Clientes c ON c.Id = a.ClienteId
INNER JOIN Veiculos v ON v.Id = a.VeiculoId
WHERE c.Id = @ClienteId;

-- 3. INNER JOIN: aluguéis abertos em uma data
SELECT a.Id, c.Nome AS Cliente, v.Modelo AS Veiculo, v.Placa, a.Inicio, a.FimPrevisto
FROM Alugueis a INNER JOIN Clientes c ON c.Id = a.ClienteId
INNER JOIN Veiculos v ON v.Id = a.VeiculoId
WHERE a.Inicio <= @Em AND (a.Devolucao IS NULL OR a.Devolucao > @Em);

-- 4. INNER JOIN: pagamentos por cliente
SELECT p.Id, p.AluguelId, c.Nome AS Cliente, p.Data, p.Valor, p.Forma
FROM Pagamentos p INNER JOIN Alugueis a ON a.Id = p.AluguelId
INNER JOIN Clientes c ON c.Id = a.ClienteId
WHERE c.Id = @ClienteId;

-- 5. LEFT JOIN: veículos sem histórico de aluguel
SELECT v.Id, v.Modelo, v.Placa, v.FabricanteId
FROM Veiculos v LEFT JOIN Alugueis a ON a.VeiculoId = v.Id
WHERE a.Id IS NULL AND (@FabricanteId IS NULL OR v.FabricanteId = @FabricanteId);
GO
