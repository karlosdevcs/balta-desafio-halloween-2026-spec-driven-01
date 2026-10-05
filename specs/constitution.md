# Constituição do projeto

## Stack

- .NET 10 e Minimal API
- Persistência em PostgreSQL 18 com EF Core (provider Npgsql)
- Testes com xUnit

## Arquitetura

- Arquitetura de camadas Domain, Application, Infra, Api
- Domain não referencia outra camada
- Endpoint não acessa banco diretamente, sempre via serviço de aplicação
- Regra de negócio não vive no endpoint nem no handler HTTP
- Nenhuma dependência nova entra sem justificativa escrita no plano
- Api só conversa com Application; Infra implementa interfaces definidas em Application

## Qualidade

- Toda regra de negócio precisa ter teste unitário
- Todo endpoint precisa ter ao menos um teste de integração de caminho feliz
- Erro de validação sempre retorna 400 com ProblemDetails
- O build não gera erros nem warnings
- Todos os testes passam antes de uma tarefa ser considerada concluída

## Convenções

- Código (classes, métodos, variáveis) em português; documentação em português
- Rotas no plural e em minúsculas (ex.: /senhas)
- JSON de entrada e saída em camelCase
- Testes nomeados como Metodo_Cenario_ResultadoEsperado
- Um tipo (classe/record) por arquivo

## Governança

- A constituição prevalece sobre spec, plan e tasks; em conflito, ela vence
- Alterar a constituição exige registrar o motivo no próprio arquivo
- Nenhum código é escrito sem uma tarefa correspondente no tasks.md
- Toda tarefa aponta para um requisito (RF/RN) ou decisão (D) de origem
- Cada tarefa é implementada isoladamente e só é concluída quando o critério "Concluída quando" é atendido
