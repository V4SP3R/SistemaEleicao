namespace SistemaEleicao.Dominio;

public sealed class EleicaoManager
{
    private readonly Dictionary<int, Candidato> _candidatos = new();

    public Candidato CadastrarCandidato(string nome, int numero)
    {
        var candidato = new Candidato(nome, numero);
        if (!_candidatos.TryAdd(numero, candidato))
            throw new InvalidOperationException("Já existe um candidato com esse número.");

        return candidato;
    }

    public IReadOnlyList<Candidato> ListarCandidatos() =>
        _candidatos.Values.OrderBy(c => c.Numero).ToList().AsReadOnly();
}
