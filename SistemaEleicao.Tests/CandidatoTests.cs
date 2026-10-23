using SistemaEleicao.Dominio;

namespace SistemaEleicao.Tests;

public class CandidatoTests
{
    [Fact]
    public void CadastrarCandidato_ComDadosValidos_ArmazenaNomeENumero()
    {
        var eleicao = new EleicaoManager();
        var candidato = eleicao.CadastrarCandidato("  Ana Silva  ", 10);

        Assert.Equal("Ana Silva", candidato.Nome);
        Assert.Equal(10, candidato.Numero);
        Assert.Same(candidato, Assert.Single(eleicao.ListarCandidatos()));
    }

    [Fact]
    public void CadastrarCandidato_ComNumeroDuplicado_RejeitaSemAlterarCadastro()
    {
        var eleicao = new EleicaoManager();
        eleicao.CadastrarCandidato("Ana", 10);

        Assert.Throws<InvalidOperationException>(() => eleicao.CadastrarCandidato("Bruno", 10));
        Assert.Equal("Ana", Assert.Single(eleicao.ListarCandidatos()).Nome);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CadastrarCandidato_SemNome_Rejeita(string? nome)
    {
        var eleicao = new EleicaoManager();
        Assert.Throws<ArgumentException>(() => eleicao.CadastrarCandidato(nome!, 10));
        Assert.Empty(eleicao.ListarCandidatos());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(99)]
    public void CadastrarCandidato_ComNumeroInvalidoOuReservado_Rejeita(int numero)
    {
        var eleicao = new EleicaoManager();
        Assert.Throws<ArgumentOutOfRangeException>(() => eleicao.CadastrarCandidato("Ana", numero));
        Assert.Empty(eleicao.ListarCandidatos());
    }

    [Fact]
    public void CadastrarCandidatos_ComNomesIguaisENumerosDistintos_Permite()
    {
        var eleicao = new EleicaoManager();
        eleicao.CadastrarCandidato("Ana", 20);
        eleicao.CadastrarCandidato("Ana", 10);

        Assert.Equal(new[] { 10, 20 }, eleicao.ListarCandidatos().Select(c => c.Numero));
    }
}
