using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text.Json;
using static System.Console;
using static userdb.ConsoleHelper;
using static userdb.Menus;

namespace userdb;

public class Program
{

    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("UserDatabase - Sistema de gestión");
        var listCommand = new Command("list", "Muestra la lista de usuarios registrados");
        listCommand.AddAlias("-l");

        listCommand.SetHandler(() =>
        {
            Commands.ListUsers();
        });

        rootCommand.AddCommand(listCommand);

        rootCommand.SetHandler(() =>
        {
            InteractiveMode();
        });

        return await rootCommand.InvokeAsync(args);
    }

    static void InteractiveMode()
    {
        UserService.EnsureDirectoryExists();
        Console.Clear();

        DrawText(" _   _ ____  _____ ____    ____    _  _____  _    ____    _    ____  _____ ", Color.Yellow);
        DrawText("| | | / ___|| ____|  _ \\  |  _ \\  / \\|_   _|/ \\  | __ )  / \\  / ___|| ____|", Color.Yellow);
        DrawText("| | | \\___ \\|  _| | |_) | | | | |/ _ \\ | | / _ \\ |  _ \\ / _ \\ \\___ \\|  _|  ", Color.DarkYellow);
        DrawText("| |_| |___) | |___|  _ <  | |_| / ___ \\| |/ ___ \\| |_) / ___ \\ ___) | |___", Color.DarkRed);
        DrawText(" \\___/|____/|_____|_| \\_\\ |____/_/   \\_\\_/_/   \\_\\____/_/   \\_\\____/|_____|", Color.Red);
        DrawText("");
        DrawText("UserDB v2.1-BETA-1 - Copyright (c) 2026 CMDPlayer216", Color.Gray);

        while (true)
        {
            DrawText("");
            DrawText("1. Mostrar usuarios");
            DrawText("2. Agregar un usuario");
            DrawText("3. Verificar un usuario");
            DrawText("4. Modificar un usuario");
            DrawText("5. Salir");
            DrawText("");

            string input = TakeInput();
            int.TryParse(input, out int option);

            try
            {
                switch (option)
                {
                    case 1:
                        ShowUsers();
                        break;
                    case 2:
                        AddUser();
                        break;
                    case 3:
                        VerifyUsers();
                        break;
                    case 4:
                        ModifyUsers();
                        break;
                    case 5:
                        return;
                    default:
                        DrawText("Esa opcion no existe!", Color.Red);
                        break;
                }
            }
            catch (Exception e)
            {
                DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
            }
        }
    }
}