using System.CommandLine;
using System.Runtime.InteropServices.Marshalling;
using NanoidDotNet;
using static userdb.ConsoleHelper;
using static userdb.Menus;

namespace userdb;

public static class Program
{

    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("UserDatabase - Sistema de gestión");

        rootCommand.SetAction((ParseResult parseResult) => { InteractiveMode(); });

        var tableOption = new Option<bool>("--table", "-t") { Description = "Muestra la salida en modo de tabla" };

        var rawOption = new Option<bool>("--raw", "-r") { Description = "Suelta todo el archivo de índice" };

        var listCommand = new Command("list", "Muestra una lista de usuarios registrados");
        listCommand.Add(tableOption);
        listCommand.Add(rawOption);

        listCommand.Validators.Add(commandResult =>
        {
            bool isTable = commandResult.GetValue(tableOption);
            bool isRaw = commandResult.GetValue(rawOption);

            if (isTable && isRaw)
            {
                commandResult.AddError("No puedes usar '--table' (-t) y '--raw' (-r) al mismo tiempo.");
            }

        });

        listCommand.SetAction((ParseResult parseResult) =>
        {
            bool isTable = parseResult.GetValue(tableOption);
            bool isRaw = parseResult.GetValue(rawOption);
            Commands.ListUsers(isTable, isRaw);
        });

        var addCommand = new Command("add", "Permite añadir usuarios");
        var nameOption = new Option<string>("--name", "-n") { Description = "Nombre del usuario a registrar", Required = true };
        var additionalRolesOption = new Option<string>("--additional-roles", "-a") { Description = "Roles adicionales que se le atribuyen", DefaultValueFactory = _ => "" };
        var fandomOption = new Option<string>("--fandom", "-f") { Description = "Fandom que se le atribuye", Required = true };
        var lookedCharactersOption = new Option<string>("--looked-characters", "-l") { Description = "Personajes que el usuario busca", DefaultValueFactory = _ => "" };
        var ageOption = new Option<int>("--age", "-A") { Description = "Edad del usuario", Required = true };
        var pronounsOption = new Option<string>("--pronouns", "-p") { Description = "Pronombres del usuario", Required = true };
        var streakOption = new Option<int>("--streak", "-s") { Description = "Racha del usuario", DefaultValueFactory = _ => 0 };
        var userIdOption = new Option<string>("--user-id", "-u") { Description = "ID del usuario", DefaultValueFactory = _ => Nanoid.Generate(size: 12) };
        var status = new Option<string>("--status", "-S") { Description = "Estado del usuario", DefaultValueFactory = _ => "Activo" };

        addCommand.Add(nameOption);
        addCommand.Add(additionalRolesOption);
        addCommand.Add(fandomOption);
        addCommand.Add(lookedCharactersOption);
        addCommand.Add(ageOption);
        addCommand.Add(pronounsOption);
        addCommand.Add(streakOption);
        addCommand.Add(userIdOption);
        addCommand.Add(status);

        addCommand.SetAction((ParseResult parseResult) =>
        {
            string name = parseResult.GetValue(nameOption) ?? "Desconocido";
            string additionalRoles = parseResult.GetValue(additionalRolesOption) ?? "";
            string fandom = parseResult.GetValue(fandomOption) ?? "Desconocido";
            string lookedCharacters = parseResult.GetValue(lookedCharactersOption) ?? "";
            string pronouns = parseResult.GetValue(pronounsOption) ?? "";
            string userid = parseResult.GetValue(userIdOption) ?? Guid.NewGuid().ToString();

            Commands.AddUser(
                string.Concat(name.Split(Path.GetInvalidFileNameChars())).Replace(",", ""),
                additionalRoles.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                fandom,
                lookedCharacters.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                parseResult.GetValue(ageOption),
                pronouns.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                parseResult.GetValue(streakOption),
                userid,
                parseResult.GetValue(status) ?? "Activo"
                );
        });

        var modifyCommand = new Command("modify", "Permite modificar un usuario");

        var nameOptionM = new Option<string>("--name", "-n") { Description = "Nuevo nombre de usuario", DefaultValueFactory = _ => "" };
        var addAdditionalRolesOption = new Option<string>("--add-additional-roles", "-a") { Description = "Añadir roles adicionales", DefaultValueFactory = _ => "" };
        var removeAdditionalRolesOption = new Option<string>("--remove-additional-roles", "-r") { Description = "Eliminar roles adicionales", DefaultValueFactory = _ => "" };
        var fandomOptionM = new Option<string>("--fandom", "-f") { Description = "Nuevo fandom que se le atribuye", DefaultValueFactory = _ => "" };
        var addLookedCharactersOption = new Option<string>("--add-looked-characters", "-l") { Description = "Añadir personajes que el usuario busca", DefaultValueFactory = _ => "" };
        var removeLookedCharactersOption = new Option<string>("--remove-looked-characters", "-L") { Description = "Eliminar personajes que el usuario busca", DefaultValueFactory = _ => "" };
        var ageOptionM = new Option<int>("--age", "-A") { Description = "Nueva edad del usuario", DefaultValueFactory = _ => 0 };
        var addPronounsOption = new Option<string>("--add-pronouns", "-p") { Description = "Añadir pronombres al usuario", DefaultValueFactory = _ => "" };
        var removePronounsOption = new Option<string>("--remove-pronouns", "-P") { Description = "Eliminar pronombres del usuario", DefaultValueFactory = _ => "" };
        var streakOptionM = new Option<int>("--streak", "-s") { Description = "Nueva racha del usuario", DefaultValueFactory = _ => 0 };
        var userIdOptionM = new Option<string>("--user-id", "-U") { Description = "Nuevo ID del usuario", DefaultValueFactory = _ => "" };
        var sourceUserOption = new Option<string>("--source-user", "-u") { Description = "Usuario que se va a modificar", Required = true };

        modifyCommand.Add(nameOptionM);
        modifyCommand.Add(addAdditionalRolesOption);
        modifyCommand.Add(removeAdditionalRolesOption);
        modifyCommand.Add(fandomOptionM);
        modifyCommand.Add(addLookedCharactersOption);
        modifyCommand.Add(removeLookedCharactersOption);
        modifyCommand.Add(ageOptionM);
        modifyCommand.Add(addPronounsOption);
        modifyCommand.Add(removePronounsOption);
        modifyCommand.Add(streakOptionM);
        modifyCommand.Add(userIdOptionM);
        modifyCommand.Add(sourceUserOption);
        modifyCommand.Add(status);



        modifyCommand.SetAction((ParseResult parseResult) =>
        {
            string additionalRolesToAdd = parseResult.GetValue(addAdditionalRolesOption) ?? "";
            string lookedCharactersToAdd = parseResult.GetValue(addLookedCharactersOption) ?? "";
            string pronounsToAdd = parseResult.GetValue(addPronounsOption) ?? "";
            string additionalRolesToRemove = parseResult.GetValue(removeAdditionalRolesOption) ?? "";
            string lookedCharactersToRemove = parseResult.GetValue(removeLookedCharactersOption) ?? "";
            string pronounsToRemove = parseResult.GetValue(removePronounsOption) ?? "";

            Commands.ModifyUser(
                parseResult.GetValue(sourceUserOption) ?? "",
                parseResult.GetValue(nameOptionM) ?? "",
                additionalRolesToAdd.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                parseResult.GetValue(fandomOptionM) ?? "",
                lookedCharactersToAdd.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                parseResult.GetValue(ageOptionM),
                pronounsToAdd.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                parseResult.GetValue(streakOptionM),
                parseResult.GetValue(userIdOptionM) ?? "",
                additionalRolesToRemove.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                lookedCharactersToRemove.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                pronounsToRemove.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                parseResult.GetValue(status) ?? "Activo"

            );
        });

        var deleteCommand = new Command("remove", "Permite eliminar un usuario");

        var noConfirm = new Option<bool>("--noconfirm") { Description = "Desactiva la confirmación de borrado (usar con cuidado)", DefaultValueFactory = _ => false };
        var userIdOptionD = new Option<string>("--user-id", "-u") { Description = "ID del usuario", Required = true };

        deleteCommand.Add(userIdOptionD);
        deleteCommand.Add(noConfirm);

        deleteCommand.SetAction((ParseResult parseResult) =>
        {
            Commands.DeleteUser(parseResult.GetValue(userIdOptionD) ?? "0", parseResult.GetValue(noConfirm));
        });

        var showCommand = new Command("show", "Muestra los datos de un usuario");

        showCommand.Add(userIdOptionD);
        showCommand.Add(rawOption);

        showCommand.SetAction((ParseResult parseResult) =>
        {
            Commands.Show(parseResult.GetValue(userIdOptionD) ?? "0", parseResult.GetValue(rawOption));
        });

        rootCommand.Add(listCommand);
        rootCommand.Add(addCommand);
        rootCommand.Add(modifyCommand);
        rootCommand.Add(deleteCommand);
        rootCommand.Add(showCommand);

        // 3. Pasar los argumentos de la aplicación al parser
        return await rootCommand.Parse(args).InvokeAsync();
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
        DrawText("UserDB v3.0 - Copyright (c) 2026 CMDPlayer216", Color.Gray);

        while (true)
        {
            DrawText("");
            DrawText("1. Mostrar usuarios");
            DrawText("2. Agregar un usuario");
            DrawText("3. Verificar un usuario");
            DrawText("4. Modificar un usuario");
            DrawText("5. Eliminar usuario");
            DrawText("6. Salir");
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
                        RemoveUser();
                        break;
                    case 6:
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