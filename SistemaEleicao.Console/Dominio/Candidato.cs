namespace SistemaEleicao.Dominio;

public sealed class Candidato
{
    public string Nome { get; }
    public int Numero { get; }

    public Candidato(string nome, int numero)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do candidato é obrigatório.", nameof(nome));

        if (numero <= 0 || numero == 99)
            throw new ArgumentOutOfRangeException(nameof(numero),
                "Use um número positivo diferente de 99. Os números 0 e 99 são reservados.");

        Nome = nome.Trim();
        Numero = numero;
    }
}
