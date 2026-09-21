using System.CommandLine;
using userdb.Commands;
using userdb.Models;
using userdb.Services;
namespace userdb.Commands.Builders;

public static class LogCommandBuilder
{
    public static Command Create()
    {
        var command = new Command("log", "Operaciones con logs");

        var liveMode = new Option<bool>("-l", "--live") { Description = "Imprime los logs a medida que se añaden" };
        var catLogs = new Option<int>("-c", "--cat") { Description = "Muestra tantas entradas en el log como se especifique (-1 = todas)", DefaultValueFactory = _ => -1 };

        command.Add(liveMode);
        command.Add(catLogs);

        command.SetAction(parseResult => LogCommand.Run(parseResult.GetValue(liveMode), parseResult.GetValue(catLogs)));

        return command;
    }
}
