# Roteiro de apresentação em vídeo (4 a 6 minutos)

**Preparação:** execute SQL Server Express, aplique a migração e inicie a API. Deixe abertas as abas `/`, `/swagger` e `/wireframes.html`. Use registros fictícios e um veículo sem histórico para demonstrar exclusão.

| Tempo aproximado | Tela | Demonstração |
|---|---|---|
| 0:00–0:30 | Wireframes | Mostre os sete quadros e diga que guiaram as telas implementadas. |
| 0:30–1:00 | Visão geral | Exiba os quatro indicadores carregados da API e os aluguéis recentes. |
| 1:00–2:00 | Fabricantes, veículos e clientes | Crie um registro, pesquise por dois campos, abra detalhes, edite e exclua um registro sem vínculo. Mostre que as outras entidades seguem o mesmo fluxo. |
| 2:00–3:00 | Aluguéis | Crie uma locação. Filtre por cliente e veículo. Demonstre a tentativa de aluguel sobreposto com resposta de conflito. Registre a devolução e mostre o total. |
| 3:00–3:35 | Pagamentos | Cadastre um pagamento, faça uma busca por forma e ID do aluguel e mostre o limite do total do aluguel. |
| 3:35–4:30 | Consultas | Execute as cinco consultas, destacando quatro `INNER JOIN` e uma `LEFT JOIN` para veículos nunca alugados. |
| 4:30–5:00 | Swagger e conclusão | Mostre as rotas do TP1 sendo reutilizadas, o front-end com `fetch`, o backend ASP.NET Core e o SQL Server Express. |

Sugestão de fala final: “O sistema mantém cadastros, aluguéis, devoluções e pagamentos em um banco relacional. O front-end consulta e altera esses dados exclusivamente pelas rotas REST do trabalho anterior.”
