using System.CommandLine;
using NanoidDotNet;
using static userdb.ConsoleHelper;
using static userdb.Menus;

namespace userdb;

public static class Program
{
    // METODO PRINCIPAL
    static async Task<int> Main(string[] args)
    {
        // Comando principal
        var rootCommand = new RootCommand("UserDatabase - Sistema de gestión");

        // En caso de que se pase sin argumentos
        rootCommand.SetAction((ParseResult parseResult) => { InteractiveMode(); });

        //Opciones para comandos                              | Opcion larga               | Op/cort | Descripcion                                                            | Valor por defecto                                     | Es requerido?     |   Comentario
        var tableOption                  = new Option<bool>(  "--table",                    "-t") {    Description = "Muestra la salida en modo de tabla" };                                                                                               // Exclusivo para comando list
        var rawOption                    = new Option<bool>(  "--raw",                      "-r") {    Description = "Da la salida sin procesar" };
        var nameOption                   = new Option<string>("--name",                     "-n") {    Description = "Nombre del usuario a registrar",                                                                                  Required = true }; // Exclusivo paa coamndo add
        var additionalRolesOption        = new Option<string>("--additional-roles",         "-a") {    Description = "Roles adicionales que se le atribuyen",                   DefaultValueFactory = _ => "" };                                           // Exclusivo paa coamndo add
        var fandomOption                 = new Option<string>("--fandom",                   "-f") {    Description = "Fandom que se le atribuye",                                                                                       Required = true }; // Exclusivo paa coamndo add
        var lookedCharactersOption       = new Option<string>("--looked-characters",        "-l") {    Description = "Personajes que el usuario busca",                         DefaultValueFactory = _ => "" };                                           // Exclusivo paa coamndo add
        var ageOption                    = new Option<int>(   "--age",                      "-A") {    Description = "Edad del usuario",                                                                                                Required = true }; // Exclusivo paa coamndo add
        var pronounsOption               = new Option<string>("--pronouns",                 "-p") {    Description = "Pronombres del usuario",                                                                                          Required = true }; // Exclusivo paa coamndo add
        var streakOption                 = new Option<int>(   "--streak",                   "-s") {    Description = "Racha del usuario",                                       DefaultValueFactory = _ => 0 };                                            // Exclusivo paa coamndo add
        Option<string>? userIdOption     = new Option<string>("--user-id",                  "-u") {    Description = "ID del usuario",                                          DefaultValueFactory = _ => Nanoid.Generate(size: 12) };                    // Exclusivo paa coamndo add
        var status                       = new Option<string>("--status",                   "-S") {    Description = "Estado del usuario",                                      DefaultValueFactory = _ => "Activo" };                                     // Exclusivo paa coamndo add
        var nameOptionM                  = new Option<string>("--name",                     "-n") {    Description = "Nuevo nombre de usuario",                                 DefaultValueFactory = _ => "" };                                           // Exclusivo del comando modify
        var addAdditionalRolesOption     = new Option<string>("--add-additional-roles",     "-a") {    Description = "Añadir roles adicionales",                                DefaultValueFactory = _ => "" };                                           // Exclusivo del comando modify
        var removeAdditionalRolesOption  = new Option<string>("--remove-additional-roles",  "-r") {    Description = "Eliminar roles adicionales",                              DefaultValueFactory = _ => "" };                                           // Exclusivo del comando modify
        var fandomOptionM                = new Option<string>("--fandom",                   "-f") {    Description = "Nuevo fandom que se le atribuye",                         DefaultValueFactory = _ => "" };                                           // Exclusivo del comando modify
        var addLookedCharactersOption    = new Option<string>("--add-looked-characters",    "-l") {    Description = "Añadir personajes que el usuario busca",                  DefaultValueFactory = _ => "" };                                           // Exclusivo del comando modify
        var removeLookedCharactersOption = new Option<string>("--remove-looked-characters", "-L") {    Description = "Eliminar personajes que el usuario busca",                DefaultValueFactory = _ => "" };                                           // Exclusivo del comando modify
        var ageOptionM                   = new Option<int>(   "--age",                      "-A") {    Description = "Nueva edad del usuario",                                  DefaultValueFactory = _ => 0 };                                            // Exclusivo del comando modify
        var addPronounsOption            = new Option<string>("--add-pronouns",             "-p") {    Description = "Añadir pronombres al usuario",                            DefaultValueFactory = _ => "" };                                           // Exclusivo del comando modify
        var removePronounsOption         = new Option<string>("--remove-pronouns",          "-P") {    Description = "Eliminar pronombres del usuario",                         DefaultValueFactory = _ => "" };                                           // Exclusivo del comando modify
        var streakOptionM                = new Option<int>(   "--streak",                   "-s") {    Description = "Nueva racha del usuario",                                 DefaultValueFactory = _ => 0 };                                            // Exclusivo del comando modify
        var userIdOptionM                = new Option<string>("--user-id",                  "-U") {    Description = "Nuevo ID del usuario",                                    DefaultValueFactory = _ => "" };                                           // Exclusivo del comando modify
        var sourceUserOption             = new Option<string>("--source-user",              "-u") {    Description = "Usuario que se va a modificar",                                                                                   Required = true };
        var noConfirm                    = new Option<bool>(  "--noconfirm")                      {    Description = "Desactiva la confirmación de borrado (usar con cuidado)", DefaultValueFactory = _ => false };
        var userIdOptionD                = new Option<string>("--user-id",                  "-u") {    Description = "ID del usuario a eliminar",                                                                                       Required = true }; // Exclusido del comando delete
        var destFileOption               = new Option<string>("--destination-file",         "-d") {    Description = "Nombre del archivo de destino",                           DefaultValueFactory = _ => ""};
        var isAllDatabaseOption          = new Option<bool>(  "--all-database",             "-a") {    Description = "Afecta a toda la base de datos"};
        var userIdOptionE                = new Option<string>("--user-id",                  "-u") {    Description = "ID del usuario a exportar"};
        var resFileOption                = new Option<string>("--resource-file",            "-r") {    Description = "Archivo de entrada",                                                                                              Required = true};
        var modeOption                   = new Option<string>("--mode",                     "-m") {    Description = "Modo de operación",                                       DefaultValueFactory = _ => "keep"};
        
        // Comandos                               | Comando  | Descripcion
        var listCommand              = new Command("list",    "Muestra una lista de usuarios registrados");
        var addCommand               = new Command("add",     "Permite añadir un usuario");
        var modifyCommand            = new Command("modify",  "Permite modificar un usuario");
        var deleteCommand            = new Command("delete",  "Permite eliminar un usuario");
        var showCommand              = new Command("show",    "Muestra los datos de un usuario");
        var exportCommand            = new Command("export",  "Operaciones de exportación");
        var importCommand            = new Command("import",  "Permite importar un usuario desde un archivo");

        // Añadir opciones a comando list
        listCommand.Add(tableOption);
        listCommand.Add(rawOption);

        // Añadir opciones a comando add
        addCommand.Add(nameOption);
        addCommand.Add(additionalRolesOption);
        addCommand.Add(fandomOption);
        addCommand.Add(lookedCharactersOption);
        addCommand.Add(ageOption);
        addCommand.Add(pronounsOption);
        addCommand.Add(streakOption);
        addCommand.Add(userIdOption);
        addCommand.Add(status);

        // Añadir opciones a comando modify
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

        // Añadir opciones a comando delete
        deleteCommand.Add(userIdOptionD);
        deleteCommand.Add(noConfirm);

        // Añadir opciones a comando show
        showCommand.Add(userIdOptionD);
        showCommand.Add(rawOption);

        // Añadir opciones a export
        exportCommand.Add(destFileOption);
        exportCommand.Add(userIdOptionE);
        exportCommand.Add(isAllDatabaseOption);

        // Aádir opciones a import
        importCommand.Add(resFileOption);
        importCommand.Add(modeOption);
        importCommand.Add(isAllDatabaseOption);

        // Validaciones para List
        listCommand.Validators.Add(commandResult =>
        {
            bool isTable = commandResult.GetValue(tableOption); // Recibimos si el usuario quiere la info en modo de tabla
            bool isRaw = commandResult.GetValue(rawOption);     // También recibimos si la quiere en formato raw (json puro en este comando)

            if (isTable && isRaw) commandResult.AddError("No puedes usar '--table' (-t) y '--raw' (-r) al mismo tiempo."); // Soltamos un error si se usan ambas al mismo tiempo

        });

        // Acciones para list
        listCommand.SetAction((ParseResult parseResult) =>
        {
            bool isTable = parseResult.GetValue(tableOption); // Recibimos si el usuario quiere la info en modo de tabla
            bool isRaw = parseResult.GetValue(rawOption);     // También recibimos si la quiere en formato raw (json puro en este comando)
            Commands.ListUsers(isTable, isRaw);               // Le pasamos el control a Commands.ListUsers() que mostrará la información según se haya especificado
        });

        // Acciones para add
        addCommand.SetAction((ParseResult parseResult) =>
        {
            string name = parseResult.GetValue(nameOption) ?? "Desconocido";                 // Limpiamos la entrada de la opción -n / --name de forma inicial
            string additionalRoles = parseResult.GetValue(additionalRolesOption) ?? "";      // Limpiamos la entrada de la opción -a / --additional-roles de forma inicial
            string fandom = parseResult.GetValue(fandomOption) ?? "Desconocido";             // Limpiamos la entrada de la opción -f / --fandom de forma inicial
            string lookedCharacters = parseResult.GetValue(lookedCharactersOption) ?? "";    // Limpiamos la entrada de la opción -l / --looked-characters de forma inicial
            string pronouns = parseResult.GetValue(pronounsOption) ?? "";                    // Limpiamos la entrada de la opción -p / --pronouns de forma inicial
            string userid = parseResult.GetValue(userIdOption) ?? Guid.NewGuid().ToString(); // Limpiamos la entrada de la opcion -u / --user-id de forma inicial

            Commands.AddUser(                                                                                        // Le pasamos el control a Commands.AddUser() que añadirá la información según se haya especificado
                string.Concat(name.Split(Path.GetInvalidFileNameChars())).Replace(",", ""),                          // Terminamos de limpiar la entrada de -n / --name
                additionalRoles.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),  // Terminamos de limpiar la entrada de -a / --aditional-roles
                fandom,                                                                                              // Pasamos la opcion -f / --fandom
                lookedCharacters.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries), // Terminamos de limpiar la entrada de -l / --looked-characters
                parseResult.GetValue(ageOption),                                                                     // Pasamos la opcion -A / --age
                pronouns.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),         // Terminamos de limpiar la entrada de -p / --pronouns
                parseResult.GetValue(streakOption),                                                                  // Pasamos la opcion -s / --streak
                userid,                                                                                              // Pasamos la opcion -u / --user-id
                parseResult.GetValue(status) ?? "Activo"                                                             // Pasamos la opcion -S / --status
                );
        });

        // Acciones para modify
        modifyCommand.SetAction((ParseResult parseResult) =>
        {
            string additionalRolesToAdd = parseResult.GetValue(addAdditionalRolesOption) ?? "";         // Limpiamos la entrada de la opción -a / --add-aditional-roles de forma inicial
            string lookedCharactersToAdd = parseResult.GetValue(addLookedCharactersOption) ?? "";       // Limpiamos la entrada de la opción -l / --add-looked-characters de forma inicial
            string pronounsToAdd = parseResult.GetValue(addPronounsOption) ?? "";                       // Limpiamos la entrada de la opción -p / --add-pronouns de forma inicial
            string additionalRolesToRemove = parseResult.GetValue(removeAdditionalRolesOption) ?? "";   // Limpiamos la entrada de la opcion -r / --remove-additional-roles de forma inicial
            string lookedCharactersToRemove = parseResult.GetValue(removeLookedCharactersOption) ?? ""; // Limpiamos la entrada de la opción -L / --remove-looked-characters de forma inicial
            string pronounsToRemove = parseResult.GetValue(removePronounsOption) ?? "";                 // Limpiamos la entrada de la opción -P / --remove-pronouns de forma inicial

            Commands.ModifyUser(                                                                                             // Le pasamos el control a Commands.ModifyUser() que modificará la información según se haya especificado
                parseResult.GetValue(sourceUserOption) ?? "",                                                                // Pasamos la opcion -u / --user-id
                parseResult.GetValue(nameOptionM) ?? "",                                                                     // Pasamos la opcion -n / --name
                additionalRolesToAdd.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),     // Terminamos de limpiar la entrada de -a / --add-aditional-roles
                parseResult.GetValue(fandomOptionM) ?? "",                                                                   // Pasamos la entrada de -f / --fandom
                lookedCharactersToAdd.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),    // Terminamos de limpiar la entrada de -l / --add-looked-characters
                parseResult.GetValue(ageOptionM),                                                                            // Pasamos la entrada de -A / --age
                pronounsToAdd.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),            // Terminamos de limpiar la entrada de -p / --pronouns
                parseResult.GetValue(streakOptionM),                                                                         // Pasamos la opcion -p / --pronouns
                parseResult.GetValue(userIdOptionM) ?? "",                                                                   // Pasamos la opcion -U / --user-id
                additionalRolesToRemove.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),  // Terminamos de limpiar la entrada de -r / --remove-additional-roles
                lookedCharactersToRemove.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries), // Terminamos de limpiar la entrada de -L / --remove-looked-characters
                pronounsToRemove.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),         // Terminamos de limpiar la opcion -P / --remove-pronouns
                parseResult.GetValue(status) ?? "Activo"                                                                     // Pasamos la opcion -S // --status
            );
        });

        // Acciones para delete
        deleteCommand.SetAction((ParseResult parseResult) =>
        {
            Commands.DeleteUser(parseResult.GetValue(userIdOptionD) ?? "0", parseResult.GetValue(noConfirm)); // Le pasamos el control a Commands.DeleteUser() que eliminará el usuario especificado
        });

        // Acciones para show
        showCommand.SetAction((ParseResult parseResult) =>
        {
            Commands.Show(parseResult.GetValue(userIdOptionD) ?? "0", parseResult.GetValue(rawOption)); // Le pasamos el control a Commands.Show() que mostrará la información según se haya especificado
        });

        // Acciones para export
        exportCommand.SetAction((ParseResult parseResult) =>
        {
            if (string.IsNullOrEmpty(parseResult.GetValue(userIdOptionE)) || parseResult.GetValue(isAllDatabaseOption)) Commands.ExportDataBase(parseResult.GetValue(destFileOption));
            if (!parseResult.GetValue(isAllDatabaseOption) && !string.IsNullOrEmpty(parseResult.GetValue(userIdOptionE))) Commands.ExportUser(parseResult.GetValue(userIdOptionE), parseResult.GetValue(destFileOption));
        });

        importCommand.SetAction((ParseResult parseResult) =>
        {
            string? source = parseResult.GetValue(resFileOption);
            string? mode = parseResult.GetValue(modeOption);

            string[] validModes = ["keep", "overwrite", "combine-keepeng-original", "combine-keeping-new"];

            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(mode))
            {
                DrawText($"Error parseando argumentos: source = {source}, mode = {mode}");
                return;
            }

            if (!validModes.Contains(mode))
            {
                DrawText("Modo no soportado", Color.Red);
                return;
            }

            if (parseResult.GetValue(isAllDatabaseOption)) Commands.ImportDataBase(source, mode);
            else Commands.ImportSingleUser(source, mode);
        });

        // Añadirmos los subcomandos al comando principal
        rootCommand.Add(listCommand);
        rootCommand.Add(addCommand);
        rootCommand.Add(modifyCommand);
        rootCommand.Add(deleteCommand);
        rootCommand.Add(showCommand);
        rootCommand.Add(exportCommand);
        rootCommand.Add(importCommand);

        // Pasar los argumentos de la aplicación al parser
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
        DrawText("UserDB v3.1 - Copyright (c) 2026 CMDPlayer216", Color.Gray);

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
            DrawText("8. Salir");
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
                        UserImportOrExport();
                        break;
                    case 7:
                        DataBaseImportOrExport();
                        break;
                    case 8:
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