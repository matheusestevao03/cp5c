# 🖱️ SetupVault API

API RESTful desenvolvida em **C# .NET 10** com **Entity Framework Core** para o **CP5 – C# Software Development**.

## 👥 Integrantes

| Nome |
|---|
| Nicolle Jelinski |
| Matheus Estevao |
| Pedro Pereira |
| João Victor Silva dos Santos |
| Eric Segawa |

## 📌 Contexto do projeto

**O que é:** o SetupVault é uma API para catalogar e controlar o estoque de periféricos de computador (mouses, mousepads, teclados, headsets, monitores etc.) e seus fabricantes.

**Qual problema resolve:** pequenas lojas e revendedores de periféricos "enthusiast" (ex.: mousepads importados da Artisan, teclados mecânicos) costumam controlar produtos em planilhas soltas, sem padronização, sem vínculo com o fabricante e sem validação de dados. Isso gera cadastros duplicados, preços inconsistentes e estoque desatualizado. A API centraliza esses dados com regras claras:

- cada periférico pertence obrigatoriamente a um fabricante existente;
- não é possível cadastrar dois fabricantes com o mesmo nome;
- um fabricante com produtos vinculados não pode ser excluído (evita registros órfãos);
- preço, estoque e tipo são validados antes de chegar ao banco.

**Para quem é destinado:** pequenas lojas e revendedores de periféricos, e também entusiastas que querem organizar o próprio setup. A API pode servir de backend para um painel administrativo, app mobile ou e-commerce.

## 🗄️ Banco de dados

**SQLite**, acessado via **Entity Framework Core 10** (`Microsoft.EntityFrameworkCore.Sqlite`).

Foi escolhido por não exigir instalação de servidor: o banco é um único arquivo (`setupvault.db`) criado automaticamente na primeira execução, o que facilita rodar o projeto em qualquer máquina.

## 🧰 Tecnologias

- .NET 10 / ASP.NET Core Web API (Controllers)
- Entity Framework Core 10 + SQLite
- Asp.Versioning (versionamento por URL `/api/v1/...`)
- Swashbuckle (Swagger UI)
- ProblemDetails + `IExceptionHandler` para tratamento global de erros
- Data Annotations para validação

## 📁 Estrutura do projeto

```
SetupVault/
├── README.md
├── .config/dotnet-tools.json        # ferramenta dotnet-ef fixada na versão 10
├── docs/evidencias/                 # prints dos testes
└── src/SetupVault.Api/
    ├── Controllers/V1/              # endpoints da versão 1
    │   ├── FabricantesController.cs
    │   └── PerifericosController.cs
    ├── Data/
    │   ├── AppDbContext.cs          # DbContext
    │   └── Configurations/          # mapeamento das entidades (Fluent API)
    ├── DTOs/                        # contratos de entrada (Request) e saída (Response)
    ├── Exceptions/                  # exceções de domínio (404, 400, 409)
    ├── Middlewares/                 # GlobalExceptionHandler
    ├── Migrations/                  # migration InitialCreate
    ├── Models/                      # entidades Fabricante e Periferico
    ├── Services/                    # regras de negócio + acesso via EF Core
    ├── Program.cs
    ├── appsettings.json
    └── SetupVault.Api.http          # roteiro de testes de todos os endpoints
```

A API segue uma separação em camadas simples: o **Controller** só recebe a requisição e devolve o status HTTP, o **Service** aplica as regras de negócio e usa o **DbContext** para persistir. Os erros são lançados como exceções de domínio e convertidos em respostas padronizadas pelo `GlobalExceptionHandler`, sem `try/catch` espalhado pelo código.

## 🧱 Modelo de dados

```
Fabricante (1) ─────────< (N) Periferico
```

**Fabricante:** `Id`, `Nome` (único, máx. 100), `PaisOrigem` (máx. 60), `SiteOficial` (opcional), `DataCadastro`

**Periferico:** `Id`, `Nome` (máx. 120), `Descricao` (opcional), `Tipo` (enum salvo como texto), `Preco` (decimal 10,2), `Estoque`, `FabricanteId` (FK), `DataCadastro`, `DataAtualizacao`

Tipos aceitos: `Mouse`, `Mousepad`, `Teclado`, `Headset`, `Monitor`, `Webcam`, `Microfone`, `Outro`.

## ▶️ Como rodar localmente

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- (Opcional) Postman, Insomnia ou a extensão REST Client do VS Code

### Passo a passo

```bash
# 1. Clonar o repositório
git clone <url-do-repositorio>
cd SetupVault

# 2. Restaurar a ferramenta do EF Core (dotnet-ef 10)
dotnet tool restore

# 3. Restaurar pacotes e compilar
dotnet build src/SetupVault.Api

# 4. Criar o banco aplicando a migration
dotnet ef database update --project src/SetupVault.Api

# 5. Rodar a API
dotnet run --project src/SetupVault.Api
```

A API sobe em **http://localhost:5080** e o Swagger abre em **http://localhost:5080/swagger**.

> O passo 4 é opcional: ao iniciar, a API também executa `Database.Migrate()` e cria o banco automaticamente se ele não existir.

## 🔄 Migration (EF Core)

O projeto possui a migration **`InitialCreate`**, localizada em `src/SetupVault.Api/Migrations/`:

| Arquivo | Função |
|---|---|
| `20260929120000_InitialCreate.cs` | Métodos `Up` (cria) e `Down` (desfaz) |
| `20260929120000_InitialCreate.Designer.cs` | Metadados do modelo naquele momento |
| `AppDbContextModelSnapshot.cs` | Snapshot atual do modelo, usado para gerar as próximas migrations |

**O que ela cria:**

- tabela `Fabricantes` com índice único `IX_Fabricantes_Nome`;
- tabela `Perifericos` com chave estrangeira `FK_Perifericos_Fabricantes_FabricanteId` (`ON DELETE RESTRICT`) e índice `IX_Perifericos_FabricanteId`.

**Comandos utilizados:**

```bash
# gerar a migration
dotnet ef migrations add InitialCreate --project src/SetupVault.Api

# aplicar no banco
dotnet ef database update --project src/SetupVault.Api

# desfazer (volta o banco ao estado vazio)
dotnet ef database update 0 --project src/SetupVault.Api
```

## 🔗 Endpoints

Base URL: `http://localhost:5080/api/v1`

### Fabricantes

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | `/api/v1/fabricantes` | Lista todos os fabricantes | 200 |
| GET | `/api/v1/fabricantes/{id}` | Busca um fabricante pelo id | 200, 404 |
| GET | `/api/v1/fabricantes/{id}/perifericos` | Lista os periféricos de um fabricante | 200, 404 |
| POST | `/api/v1/fabricantes` | Cadastra um fabricante | 201, 400, 409 |
| PUT | `/api/v1/fabricantes/{id}` | Atualiza um fabricante | 200, 400, 404, 409 |
| DELETE | `/api/v1/fabricantes/{id}` | Remove um fabricante sem periféricos | 204, 404, 409 |

### Periféricos

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | `/api/v1/perifericos` | Lista periféricos (filtros opcionais `?tipo=` e `?fabricanteId=`) | 200, 400 |
| GET | `/api/v1/perifericos/{id}` | Busca um periférico pelo id | 200, 404 |
| POST | `/api/v1/perifericos` | Cadastra um periférico | 201, 400 |
| PUT | `/api/v1/perifericos/{id}` | Atualiza um periférico | 200, 400, 404 |
| DELETE | `/api/v1/perifericos/{id}` | Remove um periférico | 204, 404 |

### Exemplos de corpo (JSON)

**POST /api/v1/fabricantes**

```json
{
  "nome": "Artisan",
  "paisOrigem": "Japão",
  "siteOficial": "https://www.artisan-jp.com"
}
```

**POST /api/v1/perifericos**

```json
{
  "nome": "Hayate Otsu XL",
  "descricao": "Mousepad de tecido, base de poron, foco em velocidade.",
  "tipo": "Mousepad",
  "preco": 389.90,
  "estoque": 15,
  "fabricanteId": 1
}
```

### Status codes utilizados

| Código | Quando acontece |
|---|---|
| 200 OK | Consulta ou atualização realizada com sucesso |
| 201 Created | Recurso criado (retorna o header `Location` com a URL do novo recurso) |
| 204 No Content | Recurso removido com sucesso |
| 400 Bad Request | Dados inválidos (validação) ou `fabricanteId` inexistente no corpo |
| 404 Not Found | Id da rota não existe |
| 409 Conflict | Nome de fabricante duplicado ou exclusão de fabricante com periféricos |
| 500 Internal Server Error | Erro inesperado (mensagem genérica, detalhes só no log) |

Todas as respostas de erro seguem o padrão **ProblemDetails**:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Recurso não encontrado",
  "status": 404,
  "detail": "Fabricante com id 999 não encontrado.",
  "instance": "/api/v1/fabricantes/999"
}
```

## 🔢 Versionamento

Implementado com **Asp.Versioning** por segmento de URL (`/api/v{version}/...`). Os controllers ficam em `Controllers/V1` e são marcados com `[ApiVersion("1.0")]`. A resposta inclui o header `api-supported-versions`, e uma futura `v2` pode ser criada em `Controllers/V2` sem quebrar os clientes da `v1`.

## 🧪 Evidências de teste

O arquivo `src/SetupVault.Api/SetupVault.Api.http` contém o roteiro completo de testes, na ordem certa para os ids baterem.

| Teste | Print |
|---|---|
| POST fabricante → 201 | ![](docs/evidencias/01-post-fabricante-201.png) |
| POST fabricante inválido → 400 | ![](docs/evidencias/02-post-fabricante-400.png) |
| POST fabricante duplicado → 409 | ![](docs/evidencias/03-post-fabricante-409.png) |
| GET fabricantes → 200 | ![](docs/evidencias/04-get-fabricantes-200.png) |
| GET fabricante por id → 200 | ![](docs/evidencias/05-get-fabricante-id-200.png) |
| GET fabricante inexistente → 404 | ![](docs/evidencias/06-get-fabricante-id-404.png) |
| PUT fabricante → 200 | ![](docs/evidencias/07-put-fabricante-200.png) |
| POST periférico → 201 | ![](docs/evidencias/08-post-periferico-201.png) |
| POST periférico com fabricante inexistente → 400 | ![](docs/evidencias/09-post-periferico-400.png) |
| GET periféricos com filtro → 200 | ![](docs/evidencias/10-get-perifericos-200.png) |
| GET periférico por id → 200 | ![](docs/evidencias/11-get-periferico-id-200.png) |
| GET periféricos do fabricante → 200 | ![](docs/evidencias/12-get-fabricante-perifericos-200.png) |
| PUT periférico → 200 | ![](docs/evidencias/13-put-periferico-200.png) |
| DELETE fabricante com periféricos → 409 | ![](docs/evidencias/14-delete-fabricante-409.png) |
| DELETE periférico → 204 | ![](docs/evidencias/15-delete-periferico-204.png) |
| DELETE periférico inexistente → 404 | ![](docs/evidencias/16-delete-periferico-404.png) |
| DELETE fabricante → 204 | ![](docs/evidencias/17-delete-fabricante-204.png) |
| Migration aplicada | ![](docs/evidencias/18-migration.png) |
