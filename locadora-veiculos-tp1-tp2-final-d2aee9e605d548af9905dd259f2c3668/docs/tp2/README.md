# Trabalho Prático 2 — Front-end integrado

O front-end está em `Locadora.Api/wwwroot/` e é servido pelo mesmo processo ASP.NET Core do Trabalho Prático 1. Assim, as chamadas `fetch('/api/...')` usam a mesma origem e as mesmas rotas já implementadas. Os dados são persistidos pelo Entity Framework Core no SQL Server Express; a interface não utiliza dados locais para simular registros.

## Wireframes

[**Abrir wireframes de alta fidelidade (link público)**](https://atlas-locadora-wireframes-2026.openfinch131072.chatgpt.site)

[Versão incluída no código-fonte](../../Locadora.Api/wwwroot/wireframes.html)

Com o projeto em execução, a URL é **`http://localhost:5080/wireframes.html`**. O arquivo HTML é independente. A versão publicada no link acima usa dados ilustrativos e pode ser enviada ao avaliador. Há sete quadros: visão geral, listagens de cadastros, formulário, aluguéis, devolução, pagamentos e consultas. Os quadros usam dados ilustrativos; a aplicação principal usa dados reais da API.

## Execução

1. Configure o SQL Server Express em `Locadora.Api/appsettings.json` ou na variável `ConnectionStrings__Locadora`.
2. Na raiz do projeto, execute `dotnet tool restore`, `dotnet restore Locadora.Api/Locadora.Api.csproj` e `dotnet tool run dotnet-ef database update --project Locadora.Api`.
3. Execute `dotnet run --project Locadora.Api --urls http://localhost:5080`.
4. Abra `http://localhost:5080/` para o front-end, `http://localhost:5080/swagger` para a API e `http://localhost:5080/wireframes.html` para o protótipo.

## Cobertura funcional

| Tela | Cadastro | Edição | Exclusão | Leitura/pesquisa | Filtros da tela | Rotas da API |
|---|---|---|---|---|---|---|
| Fabricantes | Sim | Sim | Sim | Sim | Nome e país | `GET/POST /api/fabricantes`, `GET/PUT/DELETE /api/fabricantes/{id}` |
| Veículos | Sim | Sim | Sim | Sim | Modelo e placa | `GET/POST /api/veiculos`, `GET/PUT/DELETE /api/veiculos/{id}` |
| Clientes | Sim | Sim | Sim | Sim | Nome e e-mail | `GET/POST /api/clientes`, `GET/PUT/DELETE /api/clientes/{id}` |
| Aluguéis | Sim | Sim | Sim, com restrições de histórico | Sim | Cliente e veículo | `GET/POST /api/alugueis`, `GET/PUT/DELETE /api/alugueis/{id}` |
| Pagamentos | Sim | Sim | Sim | Sim | Forma e ID do aluguel | `GET/POST /api/pagamentos`, `GET/PUT/DELETE /api/pagamentos/{id}` |
| Devolução | — | Registra devolução | — | Exibe dados do aluguel | — | `PUT /api/alugueis/{id}/devolucao` |
| Consultas | — | — | — | Sim | Parâmetro de cada consulta | Cinco rotas `GET /api/consultas/...` |

As buscas de dois campos por entidade são combinadas no navegador sobre a lista carregada pela API. A tela **Consultas** usa diretamente as cinco consultas relacionais do TP1:

1. `veiculos-por-fabricante` — `INNER JOIN`;
2. `alugueis-por-cliente` — `INNER JOIN`;
3. `alugueis-abertos` — `INNER JOIN`;
4. `pagamentos-por-cliente` — `INNER JOIN`;
5. `veiculos-sem-aluguel` — `LEFT JOIN`.

## Integração e validação

O formulário carrega opções de fabricante, cliente, veículo e aluguel pela API. Um aluguel só pode receber pagamento após devolução, conforme as regras do TP1. A interface exibe respostas `400`, `404` e `409` do backend em mensagens claras. Botões de exclusão usam confirmação. As tabelas e os resultados de consulta escapam texto recebido da API antes de inseri-lo no HTML.

**Testes neste ambiente:** compilação .NET aprovada; arquivos estáticos e Swagger responderam HTTP 200; sete verificações funcionais da interface com respostas controladas passaram. Consulte [o relatório de verificação](VERIFICACAO.md). O teste ponta a ponta com SQL Server Express e a captura do vídeo precisam ser feitos em uma máquina com a instância do banco em execução; este ambiente não conseguiu iniciar o contêiner do SQL Server Express por falta de espaço.
