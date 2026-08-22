using System.CommandLine;
using userdb.Commands;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Commands.Builders;

public static class ListCommandBuilder
{
    public static Command Create()
    {
        var command = new Command("list", "Muestra una lista de usuarios registrados");

        var isTable = new Option<bool>("--table", "-t") { Description = "Muestra la lista en formato de tabla desblozando todos los datos de cada usuario", DefaultValueFactory = _ => false };
        var isRaw = new Option<bool>("--raw", "-r") { Description = "Muestra todo el contenido del archivo índice", DefaultValueFactory = _ => false };

        command.Add(isTable);
        command.Add(isRaw);

        command.SetAction((ParseResult parseResult) =>
        {
            bool isATable = parseResult.GetValue(isTable);
            bool isInRaw = parseResult.GetValue(isRaw);

            Logs.Log("ListCommandBuilder", $"Ejecutando subcomando CLI 'list' (table: {isATable}, raw: {isInRaw})", Logs.logType.Info, 2);

            if (isATable && isInRaw)
            {
                Logs.Log("ListCommandBuilder", "Opciones conflictivas seleccionadas: --table y --raw juntas", Logs.logType.Warning, 2);
                DrawText("No puedes usar esas dos opciones juntas.", Color.Red);
            }
            else ListUsers.Run(isATable, isInRaw);
        });

        return command;
    }
}