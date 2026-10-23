# Validação da atividade

Data da execução: 23/10/2026.

## Ambiente e resultado

- SDK .NET 10.0.402, Linux x64.
- Solução e projetos compilados em Release.
- Testes xUnit: **47 aprovados, 0 reprovados e 0 ignorados**.
- Sem avisos de compilação no log da execução final.
- O programa real também foi executado por entrada redirecionada, com código
  de saída 0 e oito verificações de saída aprovadas.

Comando de teste usado no ambiente de execução:

~~~bash
dotnet test SistemaEleicao.sln -c Release -m:1 -p:UseSharedCompilation=false --disable-build-servers --nologo --logger "trx;LogFileName=resultados.trx"
~~~

As opções de processo único e compilação sem servidor contornam uma limitação
de comunicação entre processos deste ambiente. Na máquina local, o comando
normal do enunciado é suficiente:

~~~bash
dotnet test SistemaEleicao.sln
~~~

## Cobertura funcional verificada

| Grupo | Casos executados | Cenários |
| --- | ---: | --- |
| Candidatos | 9 | Nome/número, duplicidade, nome vazio, número inválido/reservado, nomes iguais |
| Eleitores e votação | 19 | Cadastro, normalização, documentos inválidos, segundo voto, eleitor inexistente, válido/branco/nulo |
| Apuração | 8 | Totais, candidato sem voto, empate, ausência de vencedor, voto rejeitado sem efeito, consulta sem mutação |
| Menu | 11 | Fluxo completo, entradas inválidas, fim da entrada, branco/nulo, segundo voto, listagem e empate |
| **Total** | **47** | **Todos aprovados** |

## Demonstração do programa

A transcrição em demonstracao-console.txt foi produzida executando o ponto
de entrada real da aplicação, sem simular a classe de interface.

Foram cadastrados Ana (10), Bruno (20) e os eleitores simulados 001, 002 e 003.
O primeiro eleitor votou em Ana; o segundo votou branco; o terceiro escolheu
77, contabilizado como nulo. A tentativa de um segundo voto do eleitor 001
foi rejeitada.

Resultado conferido: Ana 1 voto, Bruno 0, brancos 1, nulos 1, total 3;
vencedora Ana. A aplicação encerrou normalmente.

## Histórico e entrega

Os commits são reais e seguem Conventional Commits. O pacote preserva a pasta
.git e inclui um bundle como alternativa para recuperar o histórico.

O código e as instruções estão prontos. A criação/publicação do repositório
público no GitHub depende da confirmação do titular da conta.
A entrega na plataforma da disciplina deve ser feita por cada aluno,
conforme o enunciado.
