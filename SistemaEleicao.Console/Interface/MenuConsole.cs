using SistemaEleicao.Dominio;

namespace SistemaEleicao.Interface;

public sealed class MenuConsole
{
    private readonly EleicaoManager _eleicao;
    private readonly TextReader _entrada;
    private readonly TextWriter _saida;

    public MenuConsole(EleicaoManager eleicao, TextReader entrada, TextWriter saida)
    {
        _eleicao = eleicao;
        _entrada = entrada;
        _saida = saida;
    }

    public void Executar()
    {
        while (true)
        {
            _saida.WriteLine("\n=== SISTEMA DE ELEIÇÃO ===");
            _saida.WriteLine("1 - Cadastrar candidato");
            _saida.WriteLine("2 - Registrar eleitor");
            _saida.WriteLine("3 - Votar");
            _saida.WriteLine("4 - Apurar votos");
            _saida.WriteLine("5 - Listar candidatos");
            _saida.WriteLine("0 - Sair");
            try
            {
                switch (Ler("Escolha uma opção: ").Trim())
                {
                    case "1": CadastrarCandidato(); break;
                    case "2": RegistrarEleitor(); break;
                    case "3": Votar(); break;
                    case "4": ExibirApuracao(); break;
                    case "5": ListarCandidatos(); break;
                    case "0":
                        _saida.WriteLine("Sistema encerrado.");
                        return;
                    default: _saida.WriteLine("Opção inválida."); break;
                }
            }
            catch (EndOfStreamException)
            {
                _saida.WriteLine("\nEntrada encerrada.");
                return;
            }
            catch (Exception erro) when (erro is ArgumentException or InvalidOperationException)
            {
                _saida.WriteLine($"Erro: {erro.Message}");
            }
        }
    }

    private void CadastrarCandidato()
    {
        var nome = Ler("Nome do candidato: ");
        var numero = LerNumero("Número do candidato (0 e 99 são reservados): ");
        _eleicao.CadastrarCandidato(nome, numero);
        _saida.WriteLine("Candidato cadastrado com sucesso.");
    }

    private void RegistrarEleitor()
    {
        _eleicao.RegistrarEleitor(Ler("Título/CPF simulado: "));
        _saida.WriteLine("Eleitor registrado com sucesso.");
    }

    private void Votar()
    {
        ListarCandidatos();
        var documento = Ler("Título/CPF do eleitor cadastrado: ");
        var escolha = Ler("Número do candidato (0 ou ENTER = branco; 99 = nulo): ").Trim();
        int? numero = null;
        if (escolha.Length > 0)
        {
            if (!int.TryParse(escolha, out var numeroDigitado))
                throw new ArgumentException("Informe um número inteiro. Nenhum voto foi registrado.");
            numero = numeroDigitado;
        }
        var tipo = _eleicao.Votar(documento, numero);
        _saida.WriteLine(tipo switch
        {
            TipoVoto.Valido => "Voto válido registrado com sucesso.",
            TipoVoto.Branco => "Voto em branco registrado com sucesso.",
            _ => "Voto nulo registrado com sucesso."
        });
    }

    private void ListarCandidatos()
    {
        var candidatos = _eleicao.ListarCandidatos();
        _saida.WriteLine("\n--- CANDIDATOS ---");
        if (candidatos.Count == 0)
            _saida.WriteLine("Nenhum candidato cadastrado.");
        foreach (var candidato in candidatos)
            _saida.WriteLine($"{candidato.Numero} - {candidato.Nome}");
    }

    private void ExibirApuracao()
    {
        var resultado = _eleicao.ApurarVotos();
        _saida.WriteLine("\n--- APURAÇÃO DOS VOTOS ---");
        foreach (var candidato in resultado.Candidatos)
            _saida.WriteLine($"{candidato.Nome} ({candidato.Numero}): {candidato.Votos} voto(s)");
        _saida.WriteLine($"Votos válidos: {resultado.VotosValidos}");
        _saida.WriteLine($"Votos brancos: {resultado.VotosBrancos}");
        _saida.WriteLine($"Votos nulos: {resultado.VotosNulos}");
        _saida.WriteLine($"Total de votos: {resultado.TotalVotos}");
        _saida.WriteLine($"Eleitores cadastrados: {_eleicao.ListarEleitores().Count}");
        if (resultado.Vencedor is { } vencedor)
            _saida.WriteLine($"Vencedor: {vencedor.Nome} ({vencedor.Numero}), com {vencedor.Votos} voto(s).");
        else if (resultado.Empate)
            _saida.WriteLine("Empate entre: " + string.Join(", ",
                resultado.Lideres.Select(c => $"{c.Nome} ({c.Numero})")) + ".");
        else
            _saida.WriteLine("Sem vencedor: não há votos válidos.");
    }

    private string Ler(string mensagem)
    {
        _saida.Write(mensagem);
        return _entrada.ReadLine() ?? throw new EndOfStreamException();
    }

    private int LerNumero(string mensagem)
    {
        if (!int.TryParse(Ler(mensagem), out var numero))
            throw new ArgumentException("Informe um número inteiro.");
        return numero;
    }
}
