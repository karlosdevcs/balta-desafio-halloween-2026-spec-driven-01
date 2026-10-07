# Tarefas

| ID | Tarefa | Origem | Depende de | Concluída quando |
|---|---|---|---|---|
| T-01 | Criar solução `GeradorSenhas.slnx` com os projetos Domain, Application, Infra, Api, UnitTests e IntegrationTests e as referências do plano | Constituição, Arquitetura | | `dotnet build` sem erros e sem warnings |
| T-02 | Criar `PoliticaDeSenha` no Domain com limites (16/128), alfabeto e validação das regras | RN-01, RN-02, RN-03, RN-04, RN-05, D-04 | T-01 | Testes unitários cobrem senha válida, curta, longa, com espaço, sem especial, sem maiúscula, sem minúscula, sem dígito e com caractere fora do alfabeto |
| T-03 | Criar entidade `Senha` com `Criar` validando via `PoliticaDeSenha` | RF-03, RN-07, CA-05, D-04, D-06, D-11 | T-02 | Testes unitários: senha válida cria entidade com Id v7; data truncada para microssegundos; senha inválida lança exceção |
| T-04 | Criar `GeradorDeSenha` com `RandomNumberGenerator` e embaralhamento Fisher-Yates | RF-01, RN-02, RN-04, RN-06, D-02, D-03 | T-02 | Testes unitários: para os tamanhos 16, 64 e 128, 1.000 senhas geradas passam na `PoliticaDeSenha`, têm o tamanho pedido e não se repetem |
| T-05 | Criar `ISenhaRepositorio`, DTO `SenhaResposta`, exceções e `SenhaServico` (gerar e obter) | RF-02, RF-03, RF-04, RF-05, D-05, D-07, D-08, D-11 | T-03, T-04 | Testes unitários com repositório falso cobrem tamanho padrão, limites 16/128, 15 e 129 rejeitados, id inválido, id inexistente e id encontrado |
| T-06 | Criar `GeradorSenhasDbContext`, mapeamento da tabela `senhas` e `SenhaRepositorio` | RF-03, D-01, Modelo de dados | T-05 | Projeto Infra compila sem warnings |
| T-07 | Criar a migração inicial e aplicá-la na inicialização da Api | RF-03, D-10 | T-06 | Migração gerada cria a tabela `senhas` com as colunas do modelo de dados |
| T-08 | Configurar `Program.cs`: DI, ProblemDetails, `IExceptionHandler` (400/404) e JSON camelCase | RF-05, D-07, D-09, D-13, Constituição | T-05, T-06 | Api compila e inicia |
| T-09 | Endpoint `POST /senhas` | RF-01, RF-02, RF-03 | T-08 | Teste de integração retorna 201 com `Location`, senha de 16 caracteres; tamanho 15 e 129 retornam 400 ProblemDetails |
| T-10 | Endpoint `GET /senhas/{id}` | RF-04, RF-05, D-08 | T-09 | Teste de integração consulta a senha criada (200, mesmo valor); id inválido retorna 400 e id inexistente retorna 404 |
| T-11 | Documentar no README como subir o banco, rodar a Api e os testes | R-01, Definição de pronto | T-10 | `dotnet build` sem warnings e `dotnet test` com todos os testes passando |
