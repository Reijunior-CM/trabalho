# Contrato das APIs

Base: `/api`. Todas as requisições com corpo usam `Content-Type: application/json`. IDs são inteiros positivos. `400` indica entrada inválida; `404`, recurso inexistente; `409`, duplicidade ou violação de regra; `500`, falha interna. As respostas de sucesso usam JSON, exceto `204`.

| Recurso | Método e rota | Corpo/parâmetros | Sucesso | Outros códigos |
|---|---|---|---|---|
| Fabricantes | GET `/fabricantes` | nenhum | 200 lista | 500 |
| Fabricantes | GET `/fabricantes/{id}` | id na rota | 200 objeto | 404, 500 |
| Fabricantes | POST `/fabricantes` | `nome`, `pais` | 201 + Location | 400, 409, 500 |
| Fabricantes | PUT `/fabricantes/{id}` | id; `nome`, `pais` | 204 | 400, 404, 409, 500 |
| Fabricantes | DELETE `/fabricantes/{id}` | id | 204 | 404, 409, 500 |
| Veículos | GET `/veiculos` | nenhum | 200 lista | 500 |
| Veículos | GET `/veiculos/{id}` | id | 200 objeto | 404, 500 |
| Veículos | POST `/veiculos` | `fabricanteId`, `modelo`, `anoFabricacao`, `quilometragem`, `placa`, `valorDiaria` | 201 + Location | 400, 409, 500 |
| Veículos | PUT `/veiculos/{id}` | id; mesmos campos do POST | 204 | 400, 404, 409, 500 |
| Veículos | DELETE `/veiculos/{id}` | id | 204 | 404, 409, 500 |
| Clientes | GET `/clientes` | nenhum | 200 lista | 500 |
| Clientes | GET `/clientes/{id}` | id | 200 objeto | 404, 500 |
| Clientes | POST `/clientes` | `nome`, `cpf` (11 dígitos válidos), `email` | 201 + Location | 400, 409, 500 |
| Clientes | PUT `/clientes/{id}` | id; mesmos campos do POST | 204 | 400, 404, 409, 500 |
| Clientes | DELETE `/clientes/{id}` | id | 204 | 404, 409, 500 |
| Aluguéis | GET `/alugueis` | nenhum | 200 lista | 500 |
| Aluguéis | GET `/alugueis/{id}` | id | 200 objeto | 404, 500 |
| Aluguéis | POST `/alugueis` | `clienteId`, `veiculoId`, `inicio`, `fimPrevisto` | 201 + Location | 400, 409, 500 |
| Aluguéis | PUT `/alugueis/{id}` | id; mesmos campos do POST, com veículo inalterado | 204 | 400, 404, 409, 500 |
| Aluguéis | PUT `/alugueis/{id}/devolucao` | `devolucao`, `quilometragemFinal` | 200 + total | 400, 404, 409, 500 |
| Aluguéis | DELETE `/alugueis/{id}` | id | 204 | 404, 409, 500 |
| Pagamentos | GET `/pagamentos` | nenhum | 200 lista | 500 |
| Pagamentos | GET `/pagamentos/{id}` | id | 200 objeto | 404, 500 |
| Pagamentos | POST `/pagamentos` | `aluguelId`, `data`, `valor`, `forma` (Pix, Cartao, Dinheiro, Transferencia) | 201 + Location | 400, 409, 500 |
| Pagamentos | PUT `/pagamentos/{id}` | id; mesmos campos do POST | 204 | 400, 404, 409, 500 |
| Pagamentos | DELETE `/pagamentos/{id}` | id | 204 | 404, 500 |

## Filtros com joins

| Método e rota | Query string | Junção | Resposta |
|---|---|---|---|
| GET `/consultas/veiculos-por-fabricante` | `fabricanteId` | INNER: Veículo × Fabricante | 200 lista |
| GET `/consultas/alugueis-por-cliente` | `clienteId` | INNER: Aluguel × Cliente × Veículo | 200 lista |
| GET `/consultas/alugueis-abertos` | `em` em ISO 8601 | INNER: Aluguel × Cliente × Veículo | 200 lista |
| GET `/consultas/pagamentos-por-cliente` | `clienteId` | INNER: Pagamento × Aluguel × Cliente | 200 lista |
| GET `/consultas/veiculos-sem-aluguel` | `fabricanteId` opcional | LEFT: Veículo × Aluguel, filtrando ausência | 200 lista |

Cada filtro retorna `400` para parâmetro inválido e `500` para falha interna. Coleções sem resultados retornam `200 []`.

Exemplo de aluguel:

```json
{"clienteId":1,"veiculoId":1,"inicio":"2026-12-01T10:00:00","fimPrevisto":"2026-12-03T10:00:00"}
```

Exemplo de devolução:

```json
{"devolucao":"2026-12-03T09:00:00","quilometragemFinal":25120}
```
