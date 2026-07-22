using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using static System.Console;
using static userdb.ConsoleHelper;

namespace userdb;

public static class Program
{
    /// <summary>
    /// Renderiza la tabla de usuarios registrados en consola con formato multilínea.
    /// </summary>
    private static void ShowUsers()
    {
        Console.Clear();

        string[] userLines = UserService.GetUserIndexLines();

        if (userLines.Length == 0)
        {
            DrawText("No hay usuarios registrados.", Color.Red);
            return;
        }

        // Definición de anchos fijos para columnas
        const int wId = 24;
        const int wName = 25;
        const int wFandom = 25;
        const int wAge = 5;
        const int wRoles = 20;
        const int wAddRoles = 20;
        const int wPronouns = 12;
        const int wDate = 12;
        const int wStreak = 6;

        // Renderizado del encabezado
        DrawText($"{Truncate("ID", wId),-wId}", Color.White, false);
        DrawText(" | ", Color.Gray, false);
        DrawText($"{Truncate("USUARIO", wName),-wName}", Color.Green, false);
        DrawText(" | ", Color.Gray, false);
        DrawText($"{Truncate("FANDOM", wFandom),-wFandom}", Color.White, false);
        DrawText(" | ", Color.Gray, false);
        DrawText($"{Truncate("EDAD", wAge),-wAge}", Color.Green, false);
        DrawText(" | ", Color.Gray, false);
        DrawText($"{Truncate("ROLES BUSCADOS", wRoles),-wRoles}", Color.White, false);
        DrawText(" | ", Color.Gray, false);
        DrawText($"{Truncate("ROLES ADICIONALES", wAddRoles),-wAddRoles}", Color.Green, false);
        DrawText(" | ", Color.Gray, false);
        DrawText($"{Truncate("PRONOMBRES", wPronouns),-wPronouns}", Color.White, false);
        DrawText(" | ", Color.Gray, false);
        DrawText($"{Truncate("FECHA REG.", wDate),-wDate}", Color.Green, false);
        DrawText(" | ", Color.Gray, false);
        DrawText($"{Truncate("RACHA", wStreak),-wStreak}", Color.White);

        int totalWidth = wId + wName + wFandom + wAge + wRoles + wAddRoles + wPronouns + wDate + wStreak + 24;
        DrawText(new string('=', totalWidth), Color.Gray);

        // Iterar registros del índice
        for (int i = 0; i < userLines.Length; i++)
        {
            string[] userData = userLines[i].Split(',');
            if (userData.Length < 2) continue;

            string jsonFile = userData[1];

            try
            {
                User? readedUser = UserService.LoadUserFromJson(jsonFile);
                if (readedUser == null) continue;

                string[] roles = readedUser.lookedCharacters ?? Array.Empty<string>();
                string[] addRoles = readedUser.additionalRoles ?? Array.Empty<string>();
                string[] pronounsList = readedUser.pronouns ?? Array.Empty<string>();

                if (roles.Length == 0) roles = new[] { "Ninguno" };
                if (addRoles.Length == 0) addRoles = new[] { "-" };
                if (pronounsList.Length == 0) pronounsList = new[] { "-" };

                int maxLines = Math.Max(roles.Length, Math.Max(addRoles.Length, pronounsList.Length));

                for (int line = 0; line < maxLines; line++)
                {
                    string idCol = (line == 0) ? Truncate(readedUser.userId, wId) : "";
                    string nameCol = (line == 0) ? Truncate(readedUser.name, wName) : "";
                    string fandomCol = (line == 0) ? Truncate(readedUser.fandom, wFandom) : "";
                    string ageCol = (line == 0) ? readedUser.age.ToString() : "";
                    string dateCol = (line == 0) ? readedUser.dateRegistered.ToString() : "";
                    string streakCol = (line == 0) ? readedUser.streak.ToString() : "";

                    string roleCol = (line < roles.Length) ? Truncate(roles[line], wRoles) : "";
                    string addRoleCol = (line < addRoles.Length) ? Truncate(addRoles[line], wAddRoles) : "";
                    string pronounCol = (line < pronounsList.Length) ? Truncate(pronounsList[line], wPronouns) : "";

                    DrawText($"{idCol,-wId}", Color.White, false);
                    DrawText(" | ", Color.Gray, false);
                    DrawText($"{nameCol,-wName}", Color.Green, false);
                    DrawText(" | ", Color.Gray, false);
                    DrawText($"{fandomCol,-wFandom}", Color.White, false);
                    DrawText(" | ", Color.Gray, false);
                    DrawText($"{ageCol,-wAge}", Color.Green, false);
                    DrawText(" | ", Color.Gray, false);
                    DrawText($"{roleCol,-wRoles}", Color.White, false);
                    DrawText(" | ", Color.Gray, false);
                    DrawText($"{addRoleCol,-wAddRoles}", Color.Green, false);
                    DrawText(" | ", Color.Gray, false);
                    DrawText($"{pronounCol,-wPronouns}", Color.White, false);
                    DrawText(" | ", Color.Gray, false);
                    DrawText($"{dateCol,-wDate}", Color.Green, false);
                    DrawText(" | ", Color.Gray, false);
                    DrawText($"{streakCol,-wStreak}", Color.White);
                }

                DrawText(new string('-', totalWidth), Color.DarkGray);
            }
            catch (JsonException)
            {
                DrawText($"[Archivo de usuario corrupto: {Path.GetFileName(jsonFile)}]", Color.Red);
            }
        }
    }

    /// <summary>
    /// Captura interactivamente datos del usuario, los valida y delega el guardado a UserService.
    /// </summary>
    private static void AddUser()
    {
        User newUser = new User();
        Console.Clear();

        string inp = "";
        string[] inpa = Array.Empty<string>();
        int inpi = 0;
        bool warning = false;

        // --- NOMBRE ---
        while (string.IsNullOrWhiteSpace(inp))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inp = TakeInput("Ingresa el nombre: ", Color.Yellow);
            inp = string.Concat(inp.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
            if (string.IsNullOrWhiteSpace(inp)) warning = true;
        }

        newUser.name = inp;
        warning = false;
        inp = "";

        // --- ROLES ADICIONALES ---
        inpa = TakeInput("Ingresa roles adicionales (separados por comas, puede estar vacío): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0])))
            inpa = Array.Empty<string>();

        newUser.additionalRoles = inpa;

        // --- FANDOM ---
        while (string.IsNullOrWhiteSpace(inp))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inp = TakeInput("Ingresa el fandom: ", Color.Yellow);
            if (string.IsNullOrWhiteSpace(inp)) warning = true;
        }

        newUser.fandom = inp;
        warning = false;
        inp = "";

        // --- ROLES BUSCADOS ---
        inpa = TakeInput("Ingresa los roles buscados (separados por comas, puede estar vacío): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0])))
            inpa = Array.Empty<string>();

        newUser.lookedCharacters = inpa;

        // --- EDAD ---
        while (string.IsNullOrWhiteSpace(inp) || inpi == 0)
        {
            if (warning) DrawText("Esa edad no es valida!", Color.Red);
            inp = TakeInput("Ingresa la edad: ", Color.Yellow);
            if (string.IsNullOrWhiteSpace(inp)) warning = true;
            if (!string.IsNullOrWhiteSpace(inp)) int.TryParse(inp, out inpi);
        }

        newUser.age = inpi;
        warning = false;
        inp = "";

        // --- PRONOMBRES ---
        inpa = Array.Empty<string>();
        while (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0])))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inpa = TakeInput("Ingresa los pronombres (separados por comas): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0]))) warning = true;
        }

        newUser.pronouns = inpa;

        // --- RACHA ---
        string inputStreak = TakeInput("Ingresa la racha (de tenerla): ", Color.Yellow) ?? "0";
        if (!string.IsNullOrWhiteSpace(inputStreak))
            int.TryParse(inputStreak, out inpi);
        else
            inpi = 0;

        newUser.streak = inpi;

        // --- USER ID ---
        string inputId = TakeInput("Ingresa el ID (Deja vacío para autogenerar): ", Color.Yellow);
        if (string.IsNullOrWhiteSpace(inputId))
            inputId = Guid.NewGuid().ToString();

        newUser.userId = inputId;
        newUser.dateRegistered = DateOnly.FromDateTime(DateTime.Now);

        // Guardar usuario vía Servicio
        UserService.SaveUser(newUser);
        DrawText("Usuario agregado exitosamente!", Color.Green);
    }

    /// <summary>
    /// Permite seleccionar un usuario y actualizar dinámicamente su racha diaria.
    /// </summary>
    private static void VerifyUsers()
    {
        Console.Clear();

        string[] lines = UserService.GetUserIndexLines();

        if (lines.Length == 0)
        {
            DrawText("No hay usuarios registrados para verificar.", Color.Red);
            return;
        }

        DrawText("SELECCIONA UN USUARIO:", Color.White);
        DrawText("");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] userData = lines[i].Split(',');
            DrawText($"{i + 1}. {userData[0]}", Color.Yellow);
        }

        DrawText("");
        string inputSelection = TakeInput("Ingresa el numero del usuario: ");

        if (!int.TryParse(inputSelection, out int selectedIndex) || selectedIndex < 1 || selectedIndex > lines.Length)
        {
            DrawText("Seleccion invalida.", Color.Red);
            return;
        }

        string[] selectedUserData = lines[selectedIndex - 1].Split(',');
        string jsonPath = selectedUserData[1];

        User? currentUser = UserService.LoadUserFromJson(jsonPath);

        if (currentUser == null)
        {
            DrawText($"No se pudo cargar la información del usuario en ({jsonPath}).", Color.Red);
            return;
        }

        DrawText($"Usuario seleccionado: {currentUser.name}", Color.White);
        DrawText("");
        DrawText($"Racha actual: {currentUser.streak}", Color.Gray);
        DrawText("");

        string respuesta = TakeInput("Realizo la actividad de hoy? (s/n): ", Color.Yellow).ToLower();

        if (respuesta == "s")
        {
            currentUser.streak += 1;
            DrawText($"Racha incrementada! Nueva racha: {currentUser.streak}", Color.Green);
        }
        else
        {
            currentUser.streak = 0;
            DrawText("Racha reiniciada a 0.", Color.Red);
        }

        UserService.UpdateUserJson(jsonPath, currentUser);
    }

    private static void ModifyUsers()
    {
        Console.Clear();

        string[] lines = UserService.GetUserIndexLines();

        if (lines.Length == 0)
        {
            DrawText("No hay usuarios registrados para modificar.", Color.Red);
            return;
        }

        DrawText("Selecciona un usuario: ", Color.White);
        DrawText("");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] userData = lines[i].Split(',');
            DrawText($"{i + 1}. {userData[0]}", Color.Yellow);
        }
        DrawText("");
        string inputSelection = TakeInput("Ingresa el numero del usuario: ");

        if (!int.TryParse(inputSelection, out int selectedIndex) || selectedIndex < 1 || selectedIndex > lines.Length)
        {
            DrawText("Seleccion invalida.", Color.Red);
            return;
        }

        int realindex = selectedIndex - 1;

        string[] selectedUserData = lines[realindex].Split(',');
        string jsonPath = selectedUserData[1];

        User? currentUser = UserService.LoadUserFromJson(jsonPath);

        if (currentUser == null)
        {
            DrawText($"No se pudo cargar la información del usuario en ({jsonPath}).", Color.Red);
            return;
        }

        string inp = currentUser.name;
        string[] inpa = currentUser.additionalRoles;
        int inpi = 0;

        string newNameInput = TakeInput("Ingresa el nombre (deja vacío para conservar el original): ", Color.Yellow);

        if (!string.IsNullOrWhiteSpace(newNameInput))
        {
            string sanitizedName = string.Concat(newNameInput.Split(Path.GetInvalidFileNameChars())).Replace(",", "");

            string oldPath = jsonPath;
            string newPath = Path.Combine(UserService.GPath, $"{sanitizedName}.json");

            if (oldPath != newPath)
            {
                if (File.Exists(oldPath))
                {
                    File.Move(oldPath, newPath);
                }

                jsonPath = newPath;
                currentUser.name = sanitizedName;

                lines[realindex] = $"{sanitizedName},{jsonPath}";
                File.WriteAllLines(Path.Combine(UserService.GPath, "users.dat"), lines);
            }
        }

        // --- ROLES ADICIONALES ---
        string[] roles = currentUser.additionalRoles;

        bool erresable = true;

        bool loop = true;

        while (loop)
        {
            Console.Clear();

            DrawText("Roles actuales: ", Color.White);

            if (roles.Length != 0)
            {
                for (int i = 0; i < roles.Length; i++)
                {
                    DrawText($"{i + 1}. {roles[i]}");
                }
            }
            else
            {
                DrawText("No hay roles registrados");
                erresable = false;
            }

            DrawText("-------------------------------");
            DrawText("1. Eliminar rol");
            DrawText("2. Añadir rol");
            DrawText("3. Omitir");

            inp = TakeInput();

            List<string> listRoles = roles.ToList();

            switch (inp)
            {
                case "1":
                    if (!erresable)
                    {
                        DrawText("No hay roles por eliminar", Color.Red);
                        break;
                    }
                    bool warn = false;
                    inpi = 0;

                    while (inpi <= 0 || inpi > roles.Length)
                    {
                        if (warn == true) DrawText("Ese rol no existe!");
                        int.TryParse(TakeInput("Selecciona el índice del rol: "), out inpi);
                        if (inpi <= 0 || inpi > roles.Length) warn = true;
                    }
                    listRoles.RemoveAt(inpi - 1);
                    roles = listRoles.ToArray();
                    break;
                case "2":
                    listRoles.Add(TakeInput("Ingresa rol a añadir: "));
                    roles = listRoles.ToArray();
                    if (!erresable) erresable = true;
                    break;
                case "3":
                    currentUser.additionalRoles = roles;
                    loop = false;
                    break;
            }
        }


        Console.Clear();
        inp = TakeInput($"Ingresa el fandom (Actual: {currentUser.fandom}, deja vacío para conservar): ", Color.Yellow);
        if (!string.IsNullOrWhiteSpace(inp))
        {
            currentUser.fandom = inp;
        }

        // --- ROLES BUSCADOS ---
        roles = currentUser.lookedCharacters;

        erresable = true;

        loop = true;

        while (loop)
        {
            Console.Clear();

            DrawText("Roles buscados actuales: ", Color.White);

            if (roles.Length != 0)
            {
                for (int i = 0; i < roles.Length; i++)
                {
                    DrawText($"{i + 1}. {roles[i]}");
                }
            }
            else
            {
                DrawText("No hay roles registrados");
                erresable = false;
            }

            DrawText("-------------------------------");
            DrawText("1. Eliminar rol");
            DrawText("2. Añadir rol");
            DrawText("3. Omitir");

            inp = TakeInput();

            List<string> listRoles = roles.ToList();

            switch (inp)
            {
                case "1":
                    if (!erresable)
                    {
                        DrawText("No hay roles por eliminar", Color.Red);
                        break;
                    }
                    bool warn = false;
                    inpi = 0;

                    while (inpi <= 0 || inpi > roles.Length)
                    {
                        if (warn == true) DrawText("Ese rol no existe!");
                        int.TryParse(TakeInput("Selecciona el índice del rol: "), out inpi);
                        if (inpi <= 0 || inpi > roles.Length) warn = true;
                    }
                    listRoles.RemoveAt(inpi - 1);
                    roles = listRoles.ToArray();
                    break;
                case "2":
                    listRoles.Add(TakeInput("Ingresa rol a añadir: "));
                    roles = listRoles.ToArray();
                    if (!erresable) erresable = true;
                    break;
                case "3":
                    currentUser.lookedCharacters = roles;
                    loop = false;
                    break;
            }
        }

        // --- EDAD ---
        Console.Clear();
        inp = TakeInput($"Ingresa la edad (Actual: {currentUser.age}, deja vacío para conservar): ", Color.Yellow);
        if (int.TryParse(inp, out inpi) && inpi > 0)
        {
            currentUser.age = inpi;
        }

        // --- PRONOMBRES ---
        string[] pronouns = currentUser.pronouns;

        erresable = true;

        loop = true;

        while (loop)
        {
            Console.Clear();

            DrawText("Pronombres actuales: ", Color.White);

            for (int i = 0; i < pronouns.Length; i++)
            {
                DrawText($"{i + 1}. {pronouns[i]}");
            }

            DrawText("-------------------------------");
            DrawText("1. Eliminar pronombre");
            DrawText("2. Añadir pronombre");
            DrawText("3. Omitir");

            inp = TakeInput();

            List<string> listPronouns = pronouns.ToList();

            switch (inp)
            {
                case "1":
                    if (!erresable)
                    {
                        DrawText("No hay pronombre por eliminar", Color.Red);
                        break;
                    }
                    bool warn = false;
                    inpi = 0;

                    while (inpi <= 0 || inpi > pronouns.Length)
                    {
                        if (warn == true) DrawText("Ese pronombre no existe!");
                        int.TryParse(TakeInput("Selecciona el índice del pronombre: "), out inpi);
                        if (inpi <= 0 || inpi > pronouns.Length) warn = true;
                    }
                    listPronouns.RemoveAt(inpi - 1);
                    pronouns = listPronouns.ToArray();
                    break;
                case "2":
                    listPronouns.Add(TakeInput("Ingresa pronombre a añadir: "));
                    pronouns = listPronouns.ToArray();
                    if (!erresable) erresable = true;
                    break;
                case "3":
                    currentUser.pronouns = pronouns;
                    loop = false;
                    break;
            }
        }

        // --- RACHA ---
        Console.Clear();
        inp = TakeInput($"Ingresa la racha (Actual: {currentUser.streak}, deja vacío para conservar): ", Color.Yellow);
        if (int.TryParse(inp, out inpi) && inpi >= 0)
        {
            currentUser.streak = inpi;
        }

        // --- USER ID ---
        Console.Clear();
        inp = TakeInput($"Ingresa la ID (Actual: {currentUser.userId}, deja vacío para conservar): ", Color.Yellow);
        if (!string.IsNullOrWhiteSpace(inp))
        {
            currentUser.userId = inp;
        }

        // Guardar usuario vía Servicio
        UserService.UpdateUserJson(jsonPath, currentUser);
        DrawText("¡Usuario modificado exitosamente!", Color.Green);
    }

    public static void Main()
    {
        UserService.EnsureDirectoryExists();
        Console.Clear();

        DrawText(" _   _ ____  _____ ____    ____    _  _____  _    ____    _    ____  _____ ", Color.Yellow);
        DrawText("| | | / ___|| ____|  _ \\  |  _ \\  / \\|_   _|/ \\  | __ )  / \\  / ___|| ____|", Color.Yellow);
        DrawText("| | | \\___ \\|  _| | |_) | | | | |/ _ \\ | | / _ \\ |  _ \\ / _ \\ \\___ \\|  _|  ", Color.DarkYellow);
        DrawText("| |_| |___) | |___|  _ <  | |_| / ___ \\| |/ ___ \\| |_) / ___ \\ ___) | |___", Color.DarkRed);
        DrawText(" \\___/|____/|_____|_| \\_\\ |____/_/   \\_\\_/_/   \\_\\____/_/   \\_\\____/|_____|", Color.Red);
        DrawText("");
        DrawText("UserDB v2.0 - Copyright (c) 2026 CMDPlayer216", Color.Gray);

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