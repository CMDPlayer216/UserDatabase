using System.CommandLine;
using userdb.Commands;
using userdb.Services;

namespace userdb.Commands.Builders;

public static class ShowCommandBuilder
{
    public static Command Create()
    {
        var command = new Command("show", "Muestra los datos de un usuario");

        var userId = new Option<string>("--user-id", "-u")
        {
            Description = "ID del usuario a consultar",
            Required = true
        };

        var isRaw = new Option<bool>("--raw", "-r")
        {
            Description = "Muestra los datos en formato JSON",
            DefaultValueFactory = _ => false
        };

        command.Add(userId);
        command.Add(isRaw);

        command.SetAction((ParseResult parseResult) =>
        {
            string idVal = parseResult.GetValue(userId) ?? "0";
            bool rawVal = parseResult.GetValue(isRaw);
            Logs.Log("ShowCommandBuilder", $"Ejecutando subcomando CLI 'show' (userId: '{idVal}', raw: {rawVal})", Logs.logType.Info, 2);
            Show.Run(idVal, rawVal);
        });

        return command;
    }
}