using System.CommandLine;
using userdb.Commands.Builders;
using userdb.Menus;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb;

public static class Program
{
    // METODO PRINCIPAL
    static async Task<int> Main(string[] args)
    {
        Logs.Log("Main", $"Iniciando UserDB con argumentos: [{(args.Length > 0 ? string.Join(", ", args) : "sin argumentos")}]", Logs.logType.Info, 2);
        // Comando principal
        var rootCommand = new RootCommand("UserDatabase - Sistema de gestión");

        // En caso de que se pase sin argumentos
        rootCommand.SetAction(_ => InteractiveMode());

        // Añadirmos los subcomandos al comando principal
        rootCommand.Add(ListCommandBuilder.Create());
        rootCommand.Add(AddCommandBuilder.Create());
        rootCommand.Add(ModifyCommandBuilder.Create());
        rootCommand.Add(DeleteCommandBuilder.Create());
        rootCommand.Add(ShowCommandBuilder.Create());
        rootCommand.Add(ExportCommandBuilder.Create());
        rootCommand.Add(ImportCommandBuilder.Create());
        rootCommand.Add(SearchCommandBuilder.Create());
        rootCommand.Add(LogCommandBuilder.Create());

        // Pasar los argumentos de la aplicación al parser
        return await rootCommand.Parse(args).InvokeAsync();
    }

    static void InteractiveMode()
    {
        const string logTitle = "InteractiveMode";
        using var dbLock = new DatabaseLock(UserService.GPath);
        if (!dbLock.Acquire())
        {
            DrawText("ERROR: la base de datos está bloqueada.", Color.Red);
            Logs.Log(logTitle, "Base de datos bloqueada", Logs.logType.Error, 2);
            Environment.Exit(2);
        }
        RegenerateIndex.Run(UserService.GPath);
        Logs.Log(logTitle, "Modo interactivo iniciado", Logs.logType.Info, 2);
        Console.Clear();

        DrawText(" _   _ ____  _____ ____    ____    _  _____  _    ____    _    ____  _____ ", Color.Yellow);
        DrawText("| | | / ___|| ____|  _ \\  |  _ \\  / \\|_   _|/ \\  | __ )  / \\  / ___|| ____|", Color.Yellow);
        DrawText("| | | \\___ \\|  _| | |_) | | | | |/ _ \\ | | / _ \\ |  _ \\ / _ \\ \\___ \\|  _|  ", Color.DarkYellow);
        DrawText("| |_| |___) | |___|  _ <  | |_| / ___ \\| |/ ___ \\| |_) / ___ \\ ___) | |___", Color.DarkRed);
        DrawText(" \\___/|____/|_____|_| \\_\\ |____/_/   \\_\\_/_/   \\_\\____/_/   \\_\\____/|_____|", Color.Red);
        DrawText("");
        DrawText("UserDB v3.3 - Copyright (c) 2026 CMDPlayer216", Color.Gray);

        while (true)
        {
            DrawText("");
            DrawText("1. Mostrar usuarios");
            DrawText("2. Agregar un usuario");
            DrawText("3. Verificar un usuario");
            DrawText("4. Modificar un usuario");
            DrawText("5. Eliminar usuario");
            DrawText("6. Importar/Exportar usuario");
            DrawText("7. Importar/Exportar base de datos");
            DrawText("8. Buscar usuario");
            DrawText("9. Salir");
            DrawText("");

            string input = TakeInput();
            int.TryParse(input, out int option);
            Logs.Log(logTitle, $"Opción seleccionada: {option}", Logs.logType.Info, 1);

            try
            {
                switch (option)
                {
                    case 1:
                        ShowUsers.Show();
                        break;
                    case 2:
                        AddUserMenu.Show();
                        break;
                    case 3:
                        VerifyUserStreak.Show();
                        break;
                    case 4:
                        ModifyUserMenu.Show();
                        break;
                    case 5:
                        RemoveUserMenu.Show();
                        break;
                    case 6:
                        UserImportOrExportMenu.Show();
                        break;
                    case 7:
                        DataBaseImportOrExportMenu.Show();
                        break;
                    case 8:
                        SearchMenu.Show();
                        break;
                    case 9:
                        Logs.Log(logTitle, "Saliendo del modo interactivo", Logs.logType.Info, 2);
                        return;
                    default:
                        Logs.Log(logTitle, $"Opción no válida ingresada: '{input}'", Logs.logType.Warning, 2);
                        DrawText("Esa opcion no existe!", Color.Red);
                        break;
                }
            }
            catch (Exception e)
            {
                DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
                Logs.Log(e.GetType().Name, e.Message, Logs.logType.Error, 4);
            }
        }
    }
}