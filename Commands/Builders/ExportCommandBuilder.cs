using System.CommandLine;
using userdb.Commands;
using userdb.Services;

namespace userdb.Commands.Builders;

public static class ExportCommandBuilder
{
    public static Command Create()
    {
        var command = new Command("export", "Operaciones de exportación");
        var targetFile = new Option<string>("--target-file", "-t") { Description = "Archivo de destino a guardar la información", Required = true };
        var userId = new Option<string>("--user-id", "-u") { Description = "ID del usuario de referencia", DefaultValueFactory = _ => ""};
        var isAllDataBase = new Option<bool>("--all-database", "-a") { Description = "Exportar toda la base de datos", DefaultValueFactory = _ => false };

        command.Add(targetFile);
        command.Add(userId);
        command.Add(isAllDataBase);

        command.SetAction((ParseResult parseResult) =>
        {
            bool allDB = parseResult.GetValue(isAllDataBase);
            Logs.Log("ExportCommandBuilder", $"Ejecutando subcomando CLI 'export' (allDB: {allDB})", Logs.logType.Info, 2);

            if (allDB) ExportDataBase.Run(parseResult.GetValue(targetFile));
            else ExportUser.Run(parseResult.GetValue(userId), parseResult.GetValue(targetFile));
        });
        return command;
    }
}