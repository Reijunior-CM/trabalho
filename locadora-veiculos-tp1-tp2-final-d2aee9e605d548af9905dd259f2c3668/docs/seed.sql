-- Execute após aplicar a migração EF. Dados fictícios; não execute em produção.
USE LocadoraDb;
GO
IF NOT EXISTS (SELECT 1 FROM Fabricantes WHERE Nome = 'Volkswagen')
    INSERT INTO Fabricantes (Nome, Pais) VALUES ('Volkswagen', 'Alemanha');
IF NOT EXISTS (SELECT 1 FROM Fabricantes WHERE Nome = 'Fiat')
    INSERT INTO Fabricantes (Nome, Pais) VALUES ('Fiat', 'Italia');
IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cpf = '52998224725')
    INSERT INTO Clientes (Nome, Cpf, Email) VALUES ('Ana Exemplo', '52998224725', 'ana.exemplo@test.local');
IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cpf = '11144477735')
    INSERT INTO Clientes (Nome, Cpf, Email) VALUES ('Bruno Exemplo', '11144477735', 'bruno.exemplo@test.local');
IF NOT EXISTS (SELECT 1 FROM Veiculos WHERE Placa = 'ABC1D23')
    INSERT INTO Veiculos (FabricanteId, Modelo, AnoFabricacao, Quilometragem, Placa, ValorDiaria)
    VALUES ((SELECT Id FROM Fabricantes WHERE Nome = 'Volkswagen'), 'Polo', 2022, 25000, 'ABC1D23', 150.00);
IF NOT EXISTS (SELECT 1 FROM Veiculos WHERE Placa = 'DEF2G34')
    INSERT INTO Veiculos (FabricanteId, Modelo, AnoFabricacao, Quilometragem, Placa, ValorDiaria)
    VALUES ((SELECT Id FROM Fabricantes WHERE Nome = 'Fiat'), 'Argo', 2023, 12000, 'DEF2G34', 130.00);
GO
SELECT * FROM Fabricantes;
SELECT * FROM Clientes;
SELECT * FROM Veiculos;
