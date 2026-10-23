using SistemaEleicao.Dominio;

namespace SistemaEleicao.Tests;

public class VotacaoTests
{
    [Fact]
    public void RegistrarEleitor_ComDocumentoValido_CadastraSemVoto()
    {
        var eleicao = new EleicaoManager();
        var eleitor = eleicao.RegistrarEleitor("123.456.789-00");

        Assert.Equal("12345678900", eleitor.Documento);
        Assert.False(eleitor.JaVotou);
        Assert.Same(eleitor, Assert.Single(eleicao.ListarEleitores()));
    }

    [Fact]
    public void RegistrarEleitor_ComDocumentoEquivalente_RejeitaDuplicidade()
    {
        var eleicao = new EleicaoManager();
        eleicao.RegistrarEleitor("123.456.789-00");

        Assert.Throws<InvalidOperationException>(() => eleicao.RegistrarEleitor("12345678900"));
        Assert.Single(eleicao.ListarEleitores());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(".- ")]
    [InlineData("ABC123")]
    [InlineData("123/456")]
    public void RegistrarEleitor_ComDocumentoInvalido_Rejeita(string? documento)
    {
        var eleicao = new EleicaoManager();
        Assert.Throws<ArgumentException>(() => eleicao.RegistrarEleitor(documento!));
        Assert.Empty(eleicao.ListarEleitores());
    }

    [Fact]
    public void Votar_EmCandidatoCadastrado_RegistraVotoValido()
    {
        var eleicao = new EleicaoManager();
        eleicao.CadastrarCandidato("Ana", 10);
        var eleitor = eleicao.RegistrarEleitor("001");

        Assert.Equal(TipoVoto.Valido, eleicao.Votar("001", 10));
        Assert.True(eleitor.JaVotou);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(99)]
    [InlineData(77)]
    public void Votar_DuasVezesComMesmoEleitor_RejeitaSegundoVoto(int primeiroVoto)
    {
        var eleicao = new EleicaoManager();
        eleicao.CadastrarCandidato("Ana", 10);
        eleicao.RegistrarEleitor("123.456.789-00");
        eleicao.Votar("12345678900", primeiroVoto);

        Assert.Throws<InvalidOperationException>(() => eleicao.Votar("123.456.789-00", 10));
    }

    [Fact]
    public void Votar_ComEleitorNaoCadastrado_Rejeita()
    {
        var eleicao = new EleicaoManager();
        eleicao.CadastrarCandidato("Ana", 10);

        Assert.Throws<InvalidOperationException>(() => eleicao.Votar("001", 10));
        Assert.Empty(eleicao.ListarEleitores());
    }

    [Theory]
    [InlineData(99)]
    [InlineData(77)]
    [InlineData(-5)]
    public void Votar_EmNumeroInexistenteOuNulo_RetornaNulo(int numero)
    {
        var eleicao = new EleicaoManager();
        var eleitor = eleicao.RegistrarEleitor("001");

        Assert.Equal(TipoVoto.Nulo, eleicao.Votar("001", numero));
        Assert.True(eleitor.JaVotou);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void Votar_EmBranco_RetornaBranco(int? numero)
    {
        var eleicao = new EleicaoManager();
        var eleitor = eleicao.RegistrarEleitor("001");

        Assert.Equal(TipoVoto.Branco, eleicao.Votar("001", numero));
        Assert.True(eleitor.JaVotou);
    }
}
