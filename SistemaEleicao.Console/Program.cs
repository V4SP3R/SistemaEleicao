using System.Text;
using SistemaEleicao.Dominio;
using SistemaEleicao.Interface;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

new MenuConsole(new EleicaoManager(), Console.In, Console.Out).Executar();
