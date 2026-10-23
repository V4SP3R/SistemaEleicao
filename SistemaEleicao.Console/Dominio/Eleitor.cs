namespace SistemaEleicao.Dominio;

public sealed class Eleitor
{
    public string Documento { get; }
    public bool JaVotou { get; private set; }

    internal Eleitor(string documento) => Documento = NormalizarDocumento(documento);

    internal void MarcarVoto() => JaVotou = true;

    internal static string NormalizarDocumento(string documento)
    {
        if (string.IsNullOrWhiteSpace(documento))
            throw new ArgumentException("Informe um título/CPF simulado.", nameof(documento));

        if (documento.Any(c => !char.IsAsciiDigit(c) && c is not '.' and not '-' and not ' '))
            throw new ArgumentException("Use apenas dígitos, pontos, hífen e espaços no documento.", nameof(documento));

        var normalizado = new string(documento.Where(char.IsAsciiDigit).ToArray());
        if (normalizado.Length == 0)
            throw new ArgumentException("O documento deve conter ao menos um dígito.", nameof(documento));

        return normalizado;
    }
}
