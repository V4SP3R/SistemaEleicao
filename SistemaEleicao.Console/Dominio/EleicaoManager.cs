namespace SistemaEleicao.Dominio;

public sealed class EleicaoManager
{
    private readonly Dictionary<int, Candidato> _candidatos = new();
    private readonly Dictionary<string, Eleitor> _eleitores = new(StringComparer.Ordinal);
    private readonly Dictionary<int, int> _votosPorCandidato = new();
    private int _votosBrancos;
    private int _votosNulos;

    public Candidato CadastrarCandidato(string nome, int numero)
    {
        var candidato = new Candidato(nome, numero);
        if (!_candidatos.TryAdd(numero, candidato))
            throw new InvalidOperationException("Já existe um candidato com esse número.");

        return candidato;
    }

    public IReadOnlyList<Candidato> ListarCandidatos() =>
        _candidatos.Values.OrderBy(c => c.Numero).ToList().AsReadOnly();

    public Eleitor RegistrarEleitor(string documento)
    {
        var eleitor = new Eleitor(documento);
        if (!_eleitores.TryAdd(eleitor.Documento, eleitor))
            throw new InvalidOperationException("Esse eleitor já está cadastrado.");

        return eleitor;
    }

    public IReadOnlyList<Eleitor> ListarEleitores() =>
        _eleitores.Values.OrderBy(e => e.Documento, StringComparer.Ordinal).ToList().AsReadOnly();

    public TipoVoto Votar(string documento, int? numeroCandidato)
    {
        var documentoNormalizado = Eleitor.NormalizarDocumento(documento);
        if (!_eleitores.TryGetValue(documentoNormalizado, out var eleitor))
            throw new InvalidOperationException("Eleitor não cadastrado. Registre-o antes de votar.");
        if (eleitor.JaVotou)
            throw new InvalidOperationException("Esse eleitor já votou. Um segundo voto não é permitido.");

        TipoVoto tipo;
        if (numeroCandidato is null or 0)
        {
            _votosBrancos++;
            tipo = TipoVoto.Branco;
        }
        else if (_candidatos.ContainsKey(numeroCandidato.Value))
        {
            var numero = numeroCandidato.Value;
            _votosPorCandidato[numero] = _votosPorCandidato.GetValueOrDefault(numero) + 1;
            tipo = TipoVoto.Valido;
        }
        else
        {
            _votosNulos++;
            tipo = TipoVoto.Nulo;
        }

        // O controle de participação não associa o documento ao candidato escolhido.
        eleitor.MarcarVoto();
        return tipo;
    }

    public ResultadoApuracao ApurarVotos() => new(
        _candidatos.Values.Select(c => new ResultadoCandidato(
            c.Nome, c.Numero, _votosPorCandidato.GetValueOrDefault(c.Numero))),
        _votosBrancos, _votosNulos);

}
