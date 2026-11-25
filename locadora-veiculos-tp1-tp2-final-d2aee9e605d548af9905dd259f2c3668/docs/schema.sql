IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE TABLE [Clientes] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(120) NOT NULL,
        [Cpf] nvarchar(11) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE TABLE [Fabricantes] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        [Pais] nvarchar(80) NOT NULL,
        CONSTRAINT [PK_Fabricantes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE TABLE [Veiculos] (
        [Id] int NOT NULL IDENTITY,
        [FabricanteId] int NOT NULL,
        [Modelo] nvarchar(100) NOT NULL,
        [AnoFabricacao] int NOT NULL,
        [Quilometragem] int NOT NULL,
        [Placa] nvarchar(8) NOT NULL,
        [ValorDiaria] decimal(12,2) NOT NULL,
        CONSTRAINT [PK_Veiculos] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Veiculo_Ano] CHECK ([AnoFabricacao] BETWEEN 1900 AND 2100),
        CONSTRAINT [CK_Veiculo_Diaria] CHECK ([ValorDiaria] > 0),
        CONSTRAINT [CK_Veiculo_Km] CHECK ([Quilometragem] >= 0),
        CONSTRAINT [FK_Veiculos_Fabricantes_FabricanteId] FOREIGN KEY ([FabricanteId]) REFERENCES [Fabricantes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE TABLE [Alugueis] (
        [Id] int NOT NULL IDENTITY,
        [ClienteId] int NOT NULL,
        [VeiculoId] int NOT NULL,
        [Inicio] datetime2 NOT NULL,
        [FimPrevisto] datetime2 NOT NULL,
        [Devolucao] datetime2 NULL,
        [QuilometragemInicial] int NOT NULL,
        [QuilometragemFinal] int NULL,
        [ValorDiaria] decimal(12,2) NOT NULL,
        [ValorTotal] decimal(12,2) NULL,
        CONSTRAINT [PK_Alugueis] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Aluguel_Devolucao] CHECK (([Devolucao] IS NULL AND [QuilometragemFinal] IS NULL AND [ValorTotal] IS NULL) OR ([Devolucao] IS NOT NULL AND [QuilometragemFinal] IS NOT NULL AND [ValorTotal] IS NOT NULL AND [Devolucao] >= [Inicio])),
        CONSTRAINT [CK_Aluguel_Diaria] CHECK ([ValorDiaria] > 0),
        CONSTRAINT [CK_Aluguel_Km] CHECK ([QuilometragemInicial] >= 0 AND ([QuilometragemFinal] IS NULL OR [QuilometragemFinal] >= [QuilometragemInicial])),
        CONSTRAINT [CK_Aluguel_Periodo] CHECK ([FimPrevisto] > [Inicio]),
        CONSTRAINT [CK_Aluguel_Total] CHECK ([ValorTotal] IS NULL OR [ValorTotal] >= 0),
        CONSTRAINT [FK_Alugueis_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Alugueis_Veiculos_VeiculoId] FOREIGN KEY ([VeiculoId]) REFERENCES [Veiculos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE TABLE [Pagamentos] (
        [Id] int NOT NULL IDENTITY,
        [AluguelId] int NOT NULL,
        [Data] datetime2 NOT NULL,
        [Valor] decimal(12,2) NOT NULL,
        [Forma] nvarchar(30) NOT NULL,
        CONSTRAINT [PK_Pagamentos] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Pagamento_Valor] CHECK ([Valor] > 0),
        CONSTRAINT [FK_Pagamentos_Alugueis_AluguelId] FOREIGN KEY ([AluguelId]) REFERENCES [Alugueis] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Alugueis_ClienteId] ON [Alugueis] ([ClienteId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Alugueis_VeiculoId_Inicio_FimPrevisto] ON [Alugueis] ([VeiculoId], [Inicio], [FimPrevisto]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clientes_Cpf] ON [Clientes] ([Cpf]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clientes_Email] ON [Clientes] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Fabricantes_Nome] ON [Fabricantes] ([Nome]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Pagamentos_AluguelId] ON [Pagamentos] ([AluguelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Veiculos_FabricanteId] ON [Veiculos] ([FabricanteId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Veiculos_Placa] ON [Veiculos] ([Placa]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125004403_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261125004403_InitialCreate', N'8.0.11');
END;
GO

COMMIT;
GO

