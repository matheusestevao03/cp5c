# Evidências de teste

Salve aqui os prints (Swagger, Postman ou Insomnia) com **exatamente** estes nomes,
assim as imagens aparecem automaticamente no README principal.

| Arquivo | O que mostrar |
|---|---|
| `01-post-fabricante-201.png` | POST /api/v1/fabricantes → 201 Created |
| `02-post-fabricante-400.png` | POST /api/v1/fabricantes com dados inválidos → 400 |
| `03-post-fabricante-409.png` | POST /api/v1/fabricantes com nome repetido → 409 |
| `04-get-fabricantes-200.png` | GET /api/v1/fabricantes → 200 |
| `05-get-fabricante-id-200.png` | GET /api/v1/fabricantes/1 → 200 |
| `06-get-fabricante-id-404.png` | GET /api/v1/fabricantes/999 → 404 |
| `07-put-fabricante-200.png` | PUT /api/v1/fabricantes/1 → 200 |
| `08-post-periferico-201.png` | POST /api/v1/perifericos → 201 Created |
| `09-post-periferico-400.png` | POST /api/v1/perifericos com fabricanteId inexistente → 400 |
| `10-get-perifericos-200.png` | GET /api/v1/perifericos?tipo=Mousepad → 200 |
| `11-get-periferico-id-200.png` | GET /api/v1/perifericos/1 → 200 |
| `12-get-fabricante-perifericos-200.png` | GET /api/v1/fabricantes/1/perifericos → 200 |
| `13-put-periferico-200.png` | PUT /api/v1/perifericos/1 → 200 |
| `14-delete-fabricante-409.png` | DELETE /api/v1/fabricantes/1 (com periféricos) → 409 |
| `15-delete-periferico-204.png` | DELETE /api/v1/perifericos/1 → 204 |
| `16-delete-periferico-404.png` | DELETE /api/v1/perifericos/1 de novo → 404 |
| `17-delete-fabricante-204.png` | DELETE /api/v1/fabricantes/1 → 204 |
| `18-migration.png` | Terminal com `dotnet ef database update` ou pasta Migrations |

Dica: seguindo a ordem da tabela (ou do arquivo `SetupVault.Api.http`), os ids batem certinho.
