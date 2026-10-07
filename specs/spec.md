# Gerador de Senhas Fortes

## Problema

Pessoas criam senhas fracas, curtas ou previsíveis porque inventar uma senha forte "de cabeça" é difícil. Além disso, quando uma senha é gerada por uma ferramenta, ela costuma ser perdida logo em seguida, pois não há como recuperá-la depois.

## Objetivo

Oferecer um serviço que gere senhas fortes seguindo um padrão único e verificável, guarde cada senha gerada e permita recuperá-la posteriormente a partir de um identificador devolvido no momento da geração.

> Aplicação lúdica, criada apenas para aprendizado de Spec-Driven Development. Não deve ser usada em produção.

## Usuários

- **Consumidor do serviço**: pessoa ou sistema que solicita a geração de uma senha e, depois, a consulta pelo identificador recebido.

## Histórias

- **H-01**: Como consumidor, quero solicitar uma senha forte para não precisar inventá-la.
- **H-02**: Como consumidor, quero escolher o tamanho da senha para atender a exigências diferentes de cada sistema.
- **H-03**: Como consumidor, quero receber um identificador junto com a senha para poder recuperá-la depois.
- **H-04**: Como consumidor, quero consultar uma senha pelo seu identificador para reaver uma senha gerada anteriormente.

## Requisitos funcionais

- **RF-01**: O sistema gera uma senha forte quando solicitado.
- **RF-02**: O consumidor pode informar o tamanho desejado da senha; quando não informar, o sistema usa o tamanho padrão de 16 caracteres.
- **RF-03**: Toda senha gerada é armazenada, recebe um identificador único e esse identificador é devolvido junto com a senha e a data/hora de geração.
- **RF-04**: O consumidor pode consultar uma senha armazenada informando o seu identificador, recebendo a senha e a data/hora de geração.
- **RF-05**: Entradas inválidas (tamanho fora dos limites ou identificador em formato inválido) são recusadas com uma mensagem que explica o problema.

## Regras de negócio

- **RN-01**: A senha tem no mínimo 16 e no máximo 128 caracteres.
- **RN-02**: A senha contém pelo menos um caractere especial do conjunto `! @ # $ % ^ & * ( ) - _ = + [ ] { } ; : , . < > ? / | ~`.
- **RN-03**: A senha não contém espaços em branco de nenhum tipo (espaço, tabulação, quebra de linha).
- **RN-04**: A senha contém pelo menos uma letra maiúscula, uma letra minúscula e um dígito.
- **RN-05**: A senha usa apenas letras sem acento (A–Z, a–z), dígitos (0–9) e os caracteres especiais da RN-02.
- **RN-06**: A escolha e a posição de cada caractere são imprevisíveis: duas solicitações não devem produzir senhas iguais nem seguir uma ordem fixa de categorias (ex.: sempre começar com maiúscula).
- **RN-07**: O identificador é gerado pelo sistema, é único e nunca é informado pelo consumidor.
- **RN-08**: Uma senha armazenada não é alterada nem removida pelo serviço.

## Casos de borda

- **CB-01**: Tamanho não informado → gera senha com 16 caracteres.
- **CB-02**: Tamanho igual a 16 ou a 128 (limites) → aceito.
- **CB-03**: Tamanho 15 ou menor, zero ou negativo → recusado como entrada inválida.
- **CB-04**: Tamanho 129 ou maior → recusado como entrada inválida.
- **CB-05**: Tamanho não numérico (ex.: texto) → recusado como entrada inválida.
- **CB-06**: Consulta com identificador em formato inválido → recusada como entrada inválida.
- **CB-07**: Consulta com identificador válido, mas inexistente → informa que a senha não foi encontrada (diferente de entrada inválida).
- **CB-08**: Consulta com o identificador vazio (todo zero) → tratada como não encontrada, pois o sistema nunca gera esse identificador.

## Fora de escopo

- Criptografia ou hash das senhas armazenadas.
- Autenticação, autorização ou controle de quem consulta cada senha.
- Listagem, edição ou exclusão de senhas.
- Escolha das categorias de caracteres pelo consumidor (ex.: senha sem caracteres especiais).
- Interface gráfica.
- Expiração de senhas.

## Critérios de aceite

- **CA-01**: Ao solicitar uma senha sem informar o tamanho, recebo uma senha de 16 caracteres, um identificador e a data/hora de geração.
- **CA-02**: Ao solicitar uma senha de tamanho N entre 16 e 128, recebo uma senha com exatamente N caracteres.
- **CA-03**: Toda senha gerada atende simultaneamente às regras RN-01 a RN-05.
- **CA-04**: Ao solicitar uma senha com tamanho fora de 16–128, recebo um erro de entrada inválida explicando o limite, e nada é armazenado.
- **CA-05**: Ao consultar com o identificador recebido na geração, recebo exatamente a mesma senha e a mesma data/hora de geração.
- **CA-06**: Ao consultar com um identificador em formato inválido, recebo um erro de entrada inválida.
- **CA-07**: Ao consultar com um identificador válido e inexistente, recebo a resposta de "não encontrada".
