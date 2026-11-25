# Locadora de veículos — API REST

Projeto acadêmico em **C# / ASP.NET Core 8**, **Entity Framework Core**, **SQL Server Express** e **Swagger**. Atende às quatro etapas: modelagem, registros/consultas, backend e roteiro de pitch. A quinta entidade é `Pagamento`.

## Requisitos locais

- SDK .NET 8;
- SQL Server Express com acesso pela instância `localhost\\SQLEXPRESS` (Windows) ou outra instância SQL Server Express acessível;
- ferramenta `dotnet-ef` versão 8.

A string de conexão está em `Locadora.Api/appsettings.json`. Ajuste-a para sua instância. Para autenticação SQL, prefira `ConnectionStrings__Locadora` como variável de ambiente; não grave senha no repositório. O certificado é confiado somente para desenvolvimento local.

## Execução

```bash
dotnet tool restore
dotnet restore Locadora.Api/Locadora.Api.csproj
dotnet tool run dotnet-ef database update --project Locadora.Api
dotnet run --project Locadora.Api --urls http://localhost:5080
```

Abra `http://localhost:5080/swagger`. A migração inicial cria o esquema. As tabelas começam vazias. Para dados de exemplo, execute `docs/seed.sql` no SSMS depois da migração. O arquivo `docs/consultas.sql` demonstra os cinco filtros também em SQL. O script idempotente `docs/schema.sql` foi gerado da migração EF Core e pode ser inspecionado no SQL Server Management Studio.

## Estrutura

- `Locadora.Api/Domain`: cinco entidades e relacionamentos;
- `Locadora.Api/Data`: mapeamento EF, chaves, índices, restrições e migração;
- `Locadora.Api/Dtos`: contratos de entrada com validação;
- `Locadora.Api/Controllers`: 25 endpoints CRUD, devolução e cinco filtros;
- `docs/MODELO.md`: modelo conceitual e regras;
- `docs/ENDPOINTS.md`: métodos, parâmetros e códigos;
- `docs/TESTES.md`: roteiro de teste e registro de evidências;
- `docs/PITCH.md`: roteiro de apresentação.

## Regras principais

A placa, o CPF, o e-mail e o nome do fabricante são únicos. Um aluguel aberto ocupa o veículo durante seu intervalo e bloqueia novos aluguéis que comecem antes da devolução. Uma transação serializável protege a consulta de disponibilidade e a criação. A diária é capturada no início e não muda com alteração futura no preço do veículo. O total é `máximo(1, teto(horas/24)) × diária`. Pagamentos só são permitidos após a devolução e não podem exceder o total. Exclusões com histórico vinculado retornam `409`.

O exemplo prioriza as regras estudadas em sala. Para produção, acrescentar autenticação, autorização, mascaramento de CPF, fuso horário padronizado, paginação, auditoria e política de cobrança de atraso.

## Trabalho Prático 2 — Front-end

O front-end integrado está em `Locadora.Api/wwwroot` e abre na raiz da mesma aplicação: `http://localhost:5080/`. Consulte a [documentação do TP2](docs/tp2/README.md), os [wireframes publicados](https://atlas-locadora-wireframes-2026.openfinch131072.chatgpt.site) e o [roteiro de vídeo](docs/tp2/VIDEO.md). O Swagger permanece em `/swagger`.

Para repetir o teste de interface: `cd tests/ui && npm install && npm test`. Veja os [resultados observados](docs/tp2/VERIFICACAO.md).
