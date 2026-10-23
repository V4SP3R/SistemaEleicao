namespace SistemaEleicao.Dominio;

public sealed class ResultadoApuracao
{
    public IReadOnlyList<ResultadoCandidato> Candidatos { get; }
    public IReadOnlyList<ResultadoCandidato> Lideres { get; }
    public int VotosBrancos { get; }
    public int VotosNulos { get; }
    public int VotosValidos => Candidatos.Sum(c => c.Votos);
    public int TotalVotos => VotosValidos + VotosBrancos + VotosNulos;
    public bool Empate => Lideres.Count > 1;
    public ResultadoCandidato? Vencedor => Lideres.Count == 1 ? Lideres[0] : null;

    internal ResultadoApuracao(IEnumerable<ResultadoCandidato> candidatos, int brancos, int nulos)
    {
        Candidatos = candidatos.OrderByDescending(c => c.Votos).ThenBy(c => c.Numero)
            .ToList().AsReadOnly();
        VotosBrancos = brancos;
        VotosNulos = nulos;
        var maiorVotacao = Candidatos.Count == 0 ? 0 : Candidatos[0].Votos;
        Lideres = Candidatos.Where(c => maiorVotacao > 0 && c.Votos == maiorVotacao)
            .ToList().AsReadOnly();
    }
}
