# Roteiro de pitch (3 a 5 minutos)

1. **Problema (30 s):** a locadora precisa manter cadastro, controlar disponibilidade, devolução, quilometragem e pagamentos com consistência.
2. **Modelo (45 s):** mostre o diagrama de cinco entidades em `MODELO.md`, as chaves e a quinta entidade Pagamento.
3. **Arquitetura (45 s):** ASP.NET Core recebe pedidos REST; DTOs validam; controllers aplicam regras; EF Core gera consultas e persiste em SQL Server Express; Swagger documenta e permite testes.
4. **Demonstração (90 s):** no Swagger, crie fabricante, cliente e veículo; abra aluguel; tente sobrepor período (409); devolva; registre pagamento; consulte os cinco filtros.
5. **Fechamento (30 s):** destaque validação, integridade relacional, transação para disponibilidade e diferença entre INNER JOIN e LEFT JOIN.

Leve o SQL Server Express em execução, a migração aplicada e os dados do `seed.sql` carregados. Tenha as capturas do relatório de testes para o caso de falha de rede durante a apresentação.
