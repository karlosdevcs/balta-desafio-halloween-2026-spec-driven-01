# Plano técnico

## Contexto

Este plano descreve **como** implementar a especificação em [spec.md](spec.md) respeitando a [constituição](constitution.md). A API expõe dois endpoints: um que gera e armazena uma senha forte (RF-01 a RF-03) e outro que consulta uma senha pelo identificador (RF-04). Toda decisão abaixo traz a sua justificativa e o requisito ou regra que a motivou.

## Arquitetura

Solução `GeradorSenhas` em camadas, conforme a constituição:

```text
src/
├── GeradorSenhas.slnx
├── GeradorSenhas.Domain/          → entidade Senha, PoliticaDeSenha, GeradorDeSenha (sem referências)
├── GeradorSenhas.Application/     → SenhaServico, ISenhaRepositorio, DTOs, exceções de aplicação
├── GeradorSenhas.Infra/           → GeradorSenhasDbContext, mapeamento, SenhaRepositorio, migrações
├── GeradorSenhas.Api/             → Program.cs, endpoints /senhas, tratamento de erros (ProblemDetails)
├── GeradorSenhas.UnitTests/       → testes de Domain e Application
└── GeradorSenhas.IntegrationTests/→ testes HTTP ponta a ponta contra PostgreSQL real
```

Referências entre projetos:

| Projeto | Referencia |
|---|---|
| Domain | — |
| Application | Domain |
| Infra | Application, Domain |
| Api | Application, Infra (apenas para registrar DI em `Program.cs`) |
| UnitTests | Domain, Application |
| IntegrationTests | Api |

Fluxo de geração: `POST /senhas` → `SenhaServico.GerarAsync` → `GeradorDeSenha.Gerar` (Domain) → `Senha.Criar` (Domain valida com `PoliticaDeSenha`) → `ISenhaRepositorio.AdicionarAsync` (Infra) → resposta 201.

Fluxo de consulta: `GET /senhas/{id}` → `SenhaServico.ObterAsync` (valida formato do id) → `ISenhaRepositorio.ObterPorIdAsync` → 200 ou 404.

## Decisões

- **D-01 — PostgreSQL 18 via EF Core (Npgsql).** Exigido pela constituição; RF-03 exige que toda senha seja persistida. O banco roda em um container Docker (`postgres:18`) para não exigir instalação local do servidor.
- **D-02 — Geração com `System.Security.Cryptography.RandomNumberGenerator`.** RN-06 exige imprevisibilidade; `Random` não é criptograficamente seguro. É parte da BCL, portanto não adiciona dependência.
- **D-03 — Algoritmo de geração:** sorteia 1 caractere de cada categoria (maiúscula, minúscula, dígito, especial), completa o restante com sorteios do alfabeto completo e embaralha tudo com Fisher-Yates usando `RandomNumberGenerator.GetInt32`. Garante RN-02 e RN-04 por construção e RN-06 pelo embaralhamento.
- **D-04 — `PoliticaDeSenha` no Domain** concentra as regras RN-01 a RN-05 (tamanho mínimo/máximo, alfabeto, categorias obrigatórias, ausência de espaços). `Senha.Criar` a usa para garantir que nenhuma senha inválida seja construída.
- **D-05 — Tamanho padrão de 16** aplicado no `SenhaServico` quando o tamanho não é informado (RF-02, CB-01).
- **D-06 — Identificador com `Guid.CreateVersion7()`** gerado pela aplicação (RN-07). Versão 7 é ordenável no tempo, o que mantém o índice da chave primária eficiente no PostgreSQL. CB-08: `Guid.Empty` nunca é gerado, então a consulta por ele simplesmente não encontra nada.
- **D-07 — Validação no Application, tradução no Api.** O `SenhaServico` lança `ErroDeValidacaoException` (tamanho fora de 16–128, id em formato inválido) e `SenhaNaoEncontradaException`. Um `IExceptionHandler` no Api converte em `ProblemDetails` 400 e 404, respectivamente. Assim nenhuma regra fica no endpoint (constituição).
- **D-08 — Id da rota recebido como texto.** `GET /senhas/{id}` não usa a restrição `{id:guid}`, pois ela devolveria 404 para formato inválido; a constituição exige 400 em erro de validação (CB-06). O texto é validado no `SenhaServico`.
- **D-09 — Tamanho não numérico (CB-05)** é rejeitado pelo binding de JSON do ASP.NET Core; `Program.cs` habilita `AddProblemDetails()` para que a `BadHttpRequestException` vire 400 com ProblemDetails.
- **D-10 — Migrações do EF Core aplicadas na inicialização da Api** via `AplicarMigracoesAsync()`, exposto pela Infra, para que a Api não manipule o `DbContext` diretamente. App lúdico, sem pipeline de deploy; simplifica executar e testar.
- **D-13 — JSON em camelCase pelo padrão do Minimal API** (`JsonSerializerDefaults.Web`), sem configuração explícita; atende a convenção da constituição e é verificado no teste de integração do `POST /senhas`.
- **D-11 — Data/hora de geração em UTC** (`DateTimeOffset`, coluna `timestamp with time zone`) via `TimeProvider` injetado, o que permite testar com hora fixa. `Senha.Criar` trunca o valor para microssegundos, a precisão do `timestamp` do PostgreSQL, para que a data devolvida na geração seja idêntica à consultada depois (CA-05).
- **D-12 — Testes de integração com `WebApplicationFactory` + Testcontainers.** Sobe um `postgres:18` descartável por execução, garantindo teste contra o banco real exigido pela constituição sem depender de estado manual.

### Dependências (justificativa exigida pela constituição)

| Pacote | Projeto | Justificativa |
|---|---|---|
| `Npgsql.EntityFrameworkCore.PostgreSQL` | Infra | Provider EF Core do PostgreSQL (D-01) |
| `Microsoft.EntityFrameworkCore.Design` | Api | Necessário para `dotnet ef migrations add` gerar as migrações (D-10) |
| `dotnet-ef` (ferramenta local) | repositório | CLI para criar migrações (D-10) |
| `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `coverlet.collector` | testes | Pacotes do template xUnit, exigidos pela constituição |
| `Microsoft.AspNetCore.Mvc.Testing` | IntegrationTests | `WebApplicationFactory` para testes HTTP em memória (D-12) |
| `Testcontainers.PostgreSql` | IntegrationTests | PostgreSQL 18 real e isolado nos testes (D-12) |

## Modelo de dados

Entidade `Senha` (Domain) → tabela `senhas` (Infra):

| Propriedade | Coluna | Tipo PostgreSQL | Restrições |
|---|---|---|---|
| `Id` (`Guid`) | `id` | `uuid` | PK, gerado pela aplicação (D-06) |
| `Valor` (`string`) | `valor` | `varchar(128)` | NOT NULL (RN-01) |
| `CriadaEm` (`DateTimeOffset`) | `criada_em` | `timestamp with time zone` | NOT NULL (D-11) |

## Contratos

### `POST /senhas` — gera e armazena uma senha (RF-01, RF-02, RF-03)

Corpo (opcional):

```json
{ "tamanho": 24 }
```

- **201 Created** — header `Location: /senhas/{id}`

```json
{ "id": "0199b4a1-...", "senha": "k#9Tq...", "criadaEm": "2026-10-07T18:00:00+00:00" }
```

- **400 Bad Request** (ProblemDetails) — `tamanho` fora de 16–128 ou não numérico (CB-03, CB-04, CB-05)

### `GET /senhas/{id}` — consulta uma senha (RF-04)

- **200 OK** — mesmo corpo do 201
- **400 Bad Request** (ProblemDetails) — `id` em formato inválido (CB-06)
- **404 Not Found** (ProblemDetails) — `id` válido sem senha associada (CB-07, CB-08)

## Ambiente

Banco local via Docker (o desenvolvedor sobe manualmente):

```bash
docker run -d --name gerador-senhas-db \
  -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=gerador_senhas \
  -p 5432:5432 -v gerador-senhas-data:/var/lib/postgresql postgres:18
```

Connection string em `appsettings.Development.json` (`ConnectionStrings:GeradorSenhas`).

## Riscos

- **R-01 — Docker indisponível** impede rodar a Api e os testes de integração. Mitigação: documentar a instalação no README; testes unitários rodam sem Docker.
- **R-02 — Senhas em texto puro no banco.** Aceito: a spec declara criptografia fora de escopo e o app não deve ir para produção.
- **R-03 — Viés no sorteio** de caracteres. Mitigado por `RandomNumberGenerator.GetInt32`, que é uniforme (D-02).
- **R-04 — Volume do `postgres:18`** mudou para `/var/lib/postgresql`; montar no caminho antigo (`/var/lib/postgresql/data`) faz o container falhar. Mitigado pelo comando documentado acima.
