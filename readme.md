<img width="100%" alt="Halloween 2026" src="https://baltaio.blob.core.windows.net/static/images/v4/challenges/halloween-2026/banner.jpg" />

## 🎃 Halloween - Desafio 1

Oi, eu sou o [seu nome aqui] e este é o espaço onde compartilho minha jornada de aprendizado durante o desafio **Halloween 2026**, realizado pelo [balta.io](https://balta.io). 👻

Aqui você vai encontrar projetos, exercícios e códigos que estou desenvolvendo durante o desafio.

### Sobre este desafio
Neste desafio o objetivo é consolidar os fundamentos do Spec Driven Development, criando uma constituição, especificação, planejamento e tarefas manualmente para serem implementadas pela IA posteriormente.

#### O que foi implementado
- Especificações em [specs/](specs/): constituição, spec, plano técnico e tarefas
- API `GeradorSenhas` (.NET 10, Minimal API, PostgreSQL 18 + EF Core) em camadas Domain, Application, Infra e Api
- `POST /senhas` gera uma senha forte (16–128 caracteres, com maiúscula, minúscula, dígito e caractere especial, sem espaços), armazena e devolve o GUID
- `GET /senhas/{id}` consulta a senha pelo GUID (400 para GUID inválido, 404 para inexistente, ambos com ProblemDetails)
- Testes unitários (xUnit) e de integração (WebApplicationFactory + Testcontainers)

#### Como executar

Pré-requisitos: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) e [Docker Desktop](https://www.docker.com/products/docker-desktop/) em execução.

1. Suba o PostgreSQL 18:

```bash
docker run -d --name gerador-senhas-db -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=gerador_senhas -p 5432:5432 -v gerador-senhas-data:/var/lib/postgresql postgres:18
```

2. Rode a API (as migrações são aplicadas na inicialização):

```bash
cd src
dotnet run --project GeradorSenhas.Api
```

3. Teste os endpoints:

```bash
curl -i -X POST http://localhost:5080/senhas -H "Content-Type: application/json" -d "{\"tamanho\": 24}"
curl -i http://localhost:5080/senhas/{id-retornado}
```

4. Rode os testes (os de integração sobem um `postgres:18` próprio via Testcontainers, então só exigem o Docker em execução):

```bash
cd src
dotnet build
dotnet test
```

Neste processo eu aprendi:
* ✅

## Bagde
<img src="https://baltaio.blob.core.windows.net/static/images/v4/challenges/halloween-2026/01.png" width="200" />

## Problema
--

## Sobre o Halloween 2026
O desafio **Halloween 2026** consiste em implementar implementar o modelo Spec Driven Development de ponta a ponta, criando apps completas com IA.

### Veja meu progresso no desafio
[Incluir link para o repositório central]
