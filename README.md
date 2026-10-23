# Sistema de Eleição

Aplicação de console em C# para cadastrar candidatos e eleitores, registrar votos
e apurar uma eleição. Atividade de Gestão e Qualidade de Software, do professor
Daniel Henrique Matos de Paiva.

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0), não apenas o runtime.
- Git, para consultar os commits e publicar o repositório.
- Terminal, Visual Studio Code ou IDE compatível com .NET 10.
- Internet na primeira restauração dos pacotes NuGet.

A solução tradicional SistemaEleicao.sln contém os projetos SistemaEleicao.Console
e SistemaEleicao.Tests. O projeto xUnit referencia o projeto principal.

## Executar

Extraia o ZIP e abra o terminal na pasta que contém SistemaEleicao.sln.

~~~bash
dotnet --version
dotnet restore SistemaEleicao.sln
dotnet build SistemaEleicao.sln
dotnet run --project SistemaEleicao.Console/SistemaEleicao.Console.csproj
~~~

No Visual Studio, abra a solução e defina SistemaEleicao.Console como projeto de inicialização.

## Testar

~~~bash
dotnet test SistemaEleicao.sln
~~~

Para executar em Release e gerar relatório TRX:

~~~bash
dotnet test SistemaEleicao.sln -c Release --logger "trx;LogFileName=resultados.trx"
~~~

São 47 casos de teste: 9 de candidatos, 19 de eleitores/votação, 8 de apuração
e 11 de interação com o menu. Os testes do menu usam StringReader e StringWriter
para executar os fluxos sem digitação manual.

## Menu

| Opção | Operação |
| --- | --- |
| 1 | Cadastrar candidato: nome e número |
| 2 | Registrar eleitor: título/CPF simulado |
| 3 | Votar: documento cadastrado e número escolhido |
| 4 | Exibir apuração, totais e vencedor ou empate |
| 5 | Listar candidatos e seus números |
| 0 | Encerrar |

### Exemplo de demonstração

1. Na opção 1, cadastre Ana com o número 10 e Bruno com o número 20.
2. Na opção 2, registre os eleitores 001, 002 e 003, separadamente.
3. Na opção 3, informe 001 e vote em 10.
4. Na opção 3, informe 002 e vote em 0 para branco.
5. Na opção 3, informe 003 e vote em 77, um candidato inexistente.
6. Na opção 4, confira: Ana tem 1 voto, Bruno tem 0, há 1 branco e 1 nulo;
   o total é 3 e Ana é a vencedora.
7. Tente votar novamente com 001: a operação será rejeitada e o total continuará 3.

## Regras de negócio

### Candidatos

- Nome obrigatório, sem aceitar somente espaços.
- Número inteiro positivo e exclusivo.
- Os números 0 e 99 são reservados para branco e nulo.
- Nomes iguais são permitidos com números diferentes.

### Eleitores e votos

- O eleitor precisa estar cadastrado antes de votar.
- O título/CPF é simulado: aceita dígitos e os separadores ponto, hífen e espaço.
  Não verifica tamanho nem dígitos verificadores de documentos reais.
- Separadores são removidos antes de cadastrar ou consultar. Assim,
  123.456.789-00 e 12345678900 identificam o mesmo eleitor.
- Um documento não pode ser cadastrado duas vezes.
- Cada eleitor vota uma única vez, inclusive se votar branco ou nulo.
- Na escolha do voto, 0 ou ENTER registra branco.
- 99 ou qualquer inteiro sem candidato correspondente registra nulo.
- Texto não numérico e valores fora do intervalo de int são rejeitados no menu;
  o voto não é consumido e o eleitor pode tentar novamente.
- A contagem é agregada: o documento não é associado ao candidato escolhido.

### Apuração

- Mostra todos os candidatos, inclusive aqueles com zero votos.
- Totaliza votos válidos, brancos, nulos e o total geral.
- Ordena os candidatos pela votação e, em igualdade, pelo número.
- Declara vencedor somente se um candidato tiver a maior votação válida isoladamente.
- No empate da maior votação positiva, informa todos os empatados.
  O enunciado não define critério de desempate.
- Sem votos válidos, informa que não há vencedor.
- Consultar a apuração não encerra a eleição nem altera os votos.

Os dados ficam em memória e são apagados ao encerrar a aplicação. Persistência,
autenticação e abertura/encerramento formal da eleição não foram exigidos e não
fazem parte desta simulação acadêmica.

## Organização e atendimento ao enunciado

| Exigência | Implementação | Testes |
| --- | --- | --- |
| Cadastro e bloqueio de número duplicado | Dominio/Candidato.cs e EleicaoManager.cs | CandidatoTests.cs |
| Cadastro de eleitores | Dominio/Eleitor.cs e EleicaoManager.cs | VotacaoTests.cs |
| Impedir segundo voto | EleicaoManager.Votar e Eleitor.JaVotou | VotacaoTests.cs e ApuracaoTests.cs |
| Branco e candidato inexistente como nulo | EleicaoManager.Votar e TipoVoto.cs | VotacaoTests.cs e ApuracaoTests.cs |
| Totais, vencedor e empate | ResultadoApuracao.cs e ResultadoCandidato.cs | ApuracaoTests.cs |
| Interface de console | Program.cs e Interface/MenuConsole.cs | MenuConsoleTests.cs |
| Refatoração | Validação do documento na entidade Eleitor e separação domínio/interface | Suíte completa |
| Documentação | Este README | Instruções de execução, testes e entrega |
| Conventional Commits | Histórico real separado por etapa | git log --oneline |

Os arquivos de domínio e interface ficam em SistemaEleicao.Console; os testes
ficam em SistemaEleicao.Tests. Program.cs configura UTF-8 e inicia MenuConsole.
A interação foi separada da regra de negócio para permitir testes do menu.

## Histórico de desenvolvimento

~~~bash
git log --oneline
git status
~~~

Os commits usam os tipos chore, test, feat, refactor e docs, com escopos como
candidato, votacao, apuracao, console e core. Os commits de testes precedem os de
implementação, conforme a sequência do roteiro. Um commit intermediário de
testes pode referenciar uma funcionalidade implementada no commit seguinte.

Cada commit contém alterações reais. O PDF sugere dois comandos git commit
depois de um único git add; para produzir dois commits com conteúdo, os arquivos
de testes e de implementação foram adicionados separadamente.

O pacote preserva a pasta .git. Acompanha também SistemaEleicao.bundle, fora da
pasta do projeto, como cópia portátil do histórico. Se a extração descartar a
pasta .git, abra o terminal junto ao bundle e recupere o repositório:

~~~bash
git clone SistemaEleicao.bundle SistemaEleicao-recuperado
cd SistemaEleicao-recuperado
git log --oneline
~~~

## Publicação no GitHub e entrega

O repositório público de destino é [V4SP3R/SistemaEleicao](https://github.com/V4SP3R/SistemaEleicao).
Na pasta do projeto, envie os commits usando os comandos abaixo.
Se você for entregar em outra conta, crie nela um repositório público e vazio,
sem gerar README, licença ou .gitignore, e ajuste a URL.

~~~bash
git remote add origin https://github.com/V4SP3R/SistemaEleicao.git
git branch -M main
git push -u origin main
~~~

Se origin já existir, consulte git remote -v e ajuste com
git remote set-url origin URL_DO_REPOSITORIO, se necessário.

Use git push para preservar os commits. Enviar somente os arquivos pelo botão
de upload do GitHub não transfere o histórico local.

Confira a visibilidade pública, o código, o README e os commits no GitHub.
Entregue o link na plataforma da disciplina no prazo informado pelo professor.
A lista permite equipes de até seis alunos, mas cada aluno deve fazer sua
própria entrega. A equipe deve informar seus integrantes conforme a orientação
da disciplina; esses nomes não foram fornecidos para esta implementação.

## Referências técnicas

- [CLI do .NET](https://learn.microsoft.com/dotnet/core/tools/)
- [Testes C# com xUnit](https://learn.microsoft.com/dotnet/core/testing/unit-testing-csharp-with-xunit)
- [Conventional Commits 1.0.0](https://www.conventionalcommits.org/pt-br/v1.0.0/)
