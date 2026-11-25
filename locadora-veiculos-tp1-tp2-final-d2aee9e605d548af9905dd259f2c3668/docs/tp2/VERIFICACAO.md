# Verificação do projeto integrado

## Resultados obtidos neste ambiente

| Verificação | Resultado observado |
|---|---|
| Compilação da API ASP.NET Core 8 após incluir o front-end | Aprovada: 0 erros e 0 avisos |
| `GET /` | HTTP 200, `text/html` |
| `GET /app.js` | HTTP 200, `text/javascript` |
| `GET /styles.css` | HTTP 200, `text/css` |
| `GET /wireframes.html` | HTTP 200, `text/html` |
| `GET /swagger` | HTTP 200, `text/html` |
| `GET /swagger/v1/swagger.json` | HTTP 200, `application/json` |
| `POST /api/clientes` com CPF `11111111111` | HTTP 400, `{"erro":"CPF inválido."}` |
| Teste funcional da interface em DOM controlado | 7 verificações aprovadas: indicadores, filtro, carregamento de edição, cadastro, devolução e consulta |
| Link público dos wireframes | HTTP 200 |

Para repetir o teste de interface: `cd tests/ui`, `npm install` e `npm test`. Ele usa respostas controladas para validar as interações, não substitui o teste com SQL Server Express.

## Pendente

O teste ponta a ponta de CRUD com SQL Server Express não foi concluído. A imagem do SQL Server 2022 Express foi baixada, mas o contêiner não inicia com o espaço disponível no disco do ambiente. Não foram produzidos prints de chamadas Swagger com respostas do banco. O roteiro de casos permanece em `docs/TESTES.md`; execute-o com SQL Server Express ativo na máquina de apresentação. A gravação do vídeo também depende dessa execução.
