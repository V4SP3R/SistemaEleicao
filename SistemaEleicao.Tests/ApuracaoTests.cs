using SistemaEleicao.Dominio;

namespace SistemaEleicao.Tests;

public class ApuracaoTests
{
    [Fact]
    public void Apurar_ComVotosMistos_ContaTodasAsCategoriasEIdentificaVencedor()
    {
        var eleicao = CriarEleicao();
        Votar(eleicao, "001", 10);
        Votar(eleicao, "002", 10);
        Votar(eleicao, "003", 20);
        Votar(eleicao, "004", 0);
        Votar(eleicao, "005", 99);
        Votar(eleicao, "006", 77);

        var resultado = eleicao.ApurarVotos();

        Assert.Equal(6, resultado.TotalVotos);
        Assert.Equal(3, resultado.VotosValidos);
        Assert.Equal(1, resultado.VotosBrancos);
        Assert.Equal(2, resultado.VotosNulos);
        Assert.Equal(2, resultado.Candidatos.Single(c => c.Numero == 10).Votos);
        Assert.Equal(1, resultado.Candidatos.Single(c => c.Numero == 20).Votos);
        Assert.Equal(0, resultado.Candidatos.Single(c => c.Numero == 30).Votos);
        Assert.Equal(10, resultado.Vencedor!.Numero);
        Assert.False(resultado.Empate);
        Assert.Equal(resultado.TotalVotos, eleicao.ListarEleitores().Count(e => e.JaVotou));
    }

    [Fact]
    public void Apurar_ComEmpate_RetornaTodosOsLideresSemEscolherVencedor()
    {
        var eleicao = CriarEleicao();
        Votar(eleicao, "001", 10);
        Votar(eleicao, "002", 20);

        var resultado = eleicao.ApurarVotos();

        Assert.True(resultado.Empate);
        Assert.Null(resultado.Vencedor);
        Assert.Equal(new[] { 10, 20 }, resultado.Lideres.Select(c => c.Numero));
    }

    [Fact]
    public void Apurar_SemCandidatosOuVotos_NaoInventaVencedor()
    {
        var resultado = new EleicaoManager().ApurarVotos();

        Assert.Empty(resultado.Candidatos);
        Assert.Empty(resultado.Lideres);
        Assert.Null(resultado.Vencedor);
        Assert.False(resultado.Empate);
        Assert.Equal(0, resultado.TotalVotos);
    }

    [Fact]
    public void Apurar_ComCandidatosSemVotos_NaoConsideraEmpateEmZero()
    {
        var resultado = CriarEleicao().ApurarVotos();

        Assert.Equal(3, resultado.Candidatos.Count);
        Assert.All(resultado.Candidatos, c => Assert.Equal(0, c.Votos));
        Assert.Null(resultado.Vencedor);
        Assert.False(resultado.Empate);
    }

    [Fact]
    public void Apurar_SomenteBrancosENulos_NaoElegeCandidato()
    {
        var eleicao = CriarEleicao();
        Votar(eleicao, "001", 0);
        Votar(eleicao, "002", null);
        Votar(eleicao, "003", 99);

        var resultado = eleicao.ApurarVotos();

        Assert.Equal(2, resultado.VotosBrancos);
        Assert.Equal(1, resultado.VotosNulos);
        Assert.Equal(0, resultado.VotosValidos);
        Assert.Null(resultado.Vencedor);
    }

    [Fact]
    public void SegundoVotoRejeitado_NaoAlteraNenhumContador()
    {
        var eleicao = CriarEleicao();
        Votar(eleicao, "001", 20);

        Assert.Throws<InvalidOperationException>(() => eleicao.Votar("001", 10));

        var resultado = eleicao.ApurarVotos();
        Assert.Equal(1, resultado.TotalVotos);
        Assert.Equal(20, resultado.Vencedor!.Numero);
        Assert.Equal(0, resultado.Candidatos.Single(c => c.Numero == 10).Votos);
    }

    [Fact]
    public void VotoNaoAutorizado_NaoAlteraApuracao()
    {
        var eleicao = CriarEleicao();
        Assert.Throws<InvalidOperationException>(() => eleicao.Votar("999", 99));
        Assert.Equal(0, eleicao.ApurarVotos().TotalVotos);
    }

    [Fact]
    public void Apurar_Repetidamente_NaoConsomeVotosENaoMudaResultadoAnterior()
    {
        var eleicao = CriarEleicao();
        Votar(eleicao, "001", 10);
        var primeiro = eleicao.ApurarVotos();
        Assert.Equal(1, eleicao.ApurarVotos().TotalVotos);

        Votar(eleicao, "002", 20);

        Assert.Equal(1, primeiro.TotalVotos);
        Assert.Equal(0, primeiro.Candidatos.Single(c => c.Numero == 20).Votos);
        Assert.Equal(2, eleicao.ApurarVotos().TotalVotos);
    }

    private static EleicaoManager CriarEleicao()
    {
        var eleicao = new EleicaoManager();
        eleicao.CadastrarCandidato("Ana", 10);
        eleicao.CadastrarCandidato("Bruno", 20);
        eleicao.CadastrarCandidato("Carla", 30);
        return eleicao;
    }

    private static void Votar(EleicaoManager eleicao, string documento, int? numero)
    {
        eleicao.RegistrarEleitor(documento);
        eleicao.Votar(documento, numero);
    }
}
