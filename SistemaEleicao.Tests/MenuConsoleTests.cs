using SistemaEleicao.Dominio;
using SistemaEleicao.Interface;

namespace SistemaEleicao.Tests;

public class MenuConsoleTests
{
    [Fact]
    public void Executar_FluxoCompleto_CadastraVotaEExibeApuracao()
    {
        var (saida, eleicao) = Executar("1\nAna\n10\n2\n001\n3\n001\n10\n4\n0\n");
        Assert.Contains("Candidato cadastrado com sucesso.", saida);
        Assert.Contains("Eleitor registrado com sucesso.", saida);
        Assert.Contains("Voto válido registrado com sucesso.", saida);
        Assert.Contains("Vencedor: Ana (10), com 1 voto(s).", saida);
        Assert.Contains("Total de votos: 1", saida);
        Assert.Equal(1, eleicao.ApurarVotos().TotalVotos);
    }

    [Fact]
    public void Executar_NumeroNaoNumerico_ExibeErroEContinua()
    {
        var (saida, eleicao) = Executar("1\nAna\nabc\n0\n");
        Assert.Contains("Erro: Informe um número inteiro.", saida);
        Assert.Contains("Sistema encerrado.", saida);
        Assert.Empty(eleicao.ListarCandidatos());
    }

    [Fact]
    public void Executar_OpcaoInvalida_ExibeMensagemEContinua()
    {
        var (saida, _) = Executar("x\n0\n");
        Assert.Contains("Opção inválida.", saida);
        Assert.Contains("Sistema encerrado.", saida);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1\n")]
    [InlineData("2\n")]
    [InlineData("3\n")]
    public void Executar_FimDaEntrada_EncerraSemLoopInfinito(string entrada)
    {
        var (saida, _) = Executar(entrada);
        Assert.Contains("Entrada encerrada.", saida);
    }

    [Fact]
    public void Executar_BrancoENulo_ExibeContadoresSemVencedor()
    {
        var (saida, eleicao) = Executar("2\n001\n2\n002\n3\n001\n\n3\n002\n88\n4\n0\n");
        Assert.Contains("Voto em branco registrado com sucesso.", saida);
        Assert.Contains("Voto nulo registrado com sucesso.", saida);
        Assert.Contains("Votos brancos: 1", saida);
        Assert.Contains("Votos nulos: 1", saida);
        Assert.Contains("Sem vencedor: não há votos válidos.", saida);
        Assert.Equal(2, eleicao.ApurarVotos().TotalVotos);
    }

    [Fact]
    public void Executar_SegundoVoto_ExibeErroESoContaPrimeiro()
    {
        var (saida, eleicao) = Executar("2\n001\n3\n001\n0\n3\n001\n99\n4\n0\n");
        Assert.Contains("Esse eleitor já votou.", saida);
        Assert.Equal(1, eleicao.ApurarVotos().TotalVotos);
        Assert.Equal(0, eleicao.ApurarVotos().VotosNulos);
    }

    [Fact]
    public void Executar_ListarCandidatos_ExibeNomesENumeros()
    {
        var (saida, _) = Executar("1\nAna\n10\n5\n0\n");
        Assert.Contains("10 - Ana", saida);
    }

    [Fact]
    public void Executar_Empate_ExibeLideresSemDeclararVencedor()
    {
        var (saida, _) = Executar("1\nAna\n10\n1\nBruno\n20\n2\n001\n2\n002\n3\n001\n10\n3\n002\n20\n4\n0\n");
        Assert.Contains("Empate entre: Ana (10), Bruno (20).", saida);
        Assert.DoesNotContain("Vencedor:", saida);
    }

    private static (string Saida, EleicaoManager Eleicao) Executar(string entrada)
    {
        var eleicao = new EleicaoManager();
        using var leitor = new StringReader(entrada);
        using var escritor = new StringWriter();
        new MenuConsole(eleicao, leitor, escritor).Executar();
        return (escritor.ToString(), eleicao);
    }
}
