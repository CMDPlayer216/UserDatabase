using System.CommandLine;
using userdb.Commands;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Commands.Builders;

public static class ImportCommandBuilder
{
    public static Command Create()
    {
        var command = new Command("import", "Operaciones de importación");
        var sourceFile = new Option<string>("--source-file", "-t") { Description = "Archivo fuente del cual importar la información", Required = true };
        var mode = new Option<string>("--mode", "-m") { Description = "Modo de importación", DefaultValueFactory = _ => ""};
        var isAllDataBase = new Option<bool>("--all-database", "-a") { Description = "Exportar toda la base de datos", DefaultValueFactory = _ => false };

        command.Add(sourceFile);
        command.Add(mode);
        command.Add(isAllDataBase);

        command.SetAction((ParseResult parseResult) =>
        {
            string? source = parseResult.GetValue(sourceFile);
            string? md = parseResult.GetValue(mode);
            bool allDB = parseResult.GetValue(isAllDataBase);

            Logs.Log("ImportCommandBuilder", $"Ejecutando subcomando CLI 'import' (source: '{source}', mode: '{md}', allDB: {allDB})", Logs.logType.Info, 2);

            string[] validModes = ["keep", "overwrite", "combine-keeping-original", "combine-keeping-new"];

            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(md))
            {
                Logs.Log("ImportCommandBuilder", $"Error parseando argumentos: source = {source}, mode = {md}", Logs.logType.Error, 3);
                DrawText($"Error parseando argumentos: source = {source}, mode = {md}");
                return;
            }

            if (!validModes.Contains(md))
            {
                Logs.Log("ImportCommandBuilder", $"Modo no soportado: '{md}'", Logs.logType.Warning, 2);
                DrawText("Modo no soportado", Color.Red);
                return;
            }

            if (allDB) ImportDataBase.Run(source, md);
            else ImportUser.Run(source, md);
        });
        
        return command;
    }
}