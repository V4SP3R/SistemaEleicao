namespace SistemaEleicao.Dominio;

public sealed class Eleitor
{
    public string Documento { get; }
    public bool JaVotou { get; private set; }

    internal Eleitor(string documento) => Documento = documento;

    internal void MarcarVoto() => JaVotou = true;
}
