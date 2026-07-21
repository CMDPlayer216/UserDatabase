using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace _userdatabase.UserDatabase;

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
            ConsoleHelper.DrawText("No hay usuarios registrados.", Color.Red);
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
        ConsoleHelper.DrawText($"{ConsoleHelper.Truncate("ID", wId),-wId}", Color.White, false);
        ConsoleHelper.DrawText(" | ", Color.Gray, false);
        ConsoleHelper.DrawText($"{ConsoleHelper.Truncate("USUARIO", wName),-wName}", Color.Green, false);
        ConsoleHelper.DrawText(" | ", Color.Gray, false);
        ConsoleHelper.DrawText($"{ConsoleHelper.Truncate("FANDOM", wFandom),-wFandom}", Color.White, false);
        ConsoleHelper.DrawText(" | ", Color.Gray, false);
        ConsoleHelper.DrawText($"{ConsoleHelper.Truncate("EDAD", wAge),-wAge}", Color.Green, false);
        ConsoleHelper.DrawText(" | ", Color.Gray, false);
        ConsoleHelper.DrawText($"{ConsoleHelper.Truncate("ROLES BUSCADOS", wRoles),-wRoles}", Color.White, false);
        ConsoleHelper.DrawText(" | ", Color.Gray, false);
        ConsoleHelper.DrawText($"{ConsoleHelper.Truncate("ROLES ADICIONALES", wAddRoles),-wAddRoles}", Color.Green, false);
        ConsoleHelper.DrawText(" | ", Color.Gray, false);
        ConsoleHelper.DrawText($"{ConsoleHelper.Truncate("PRONOMBRES", wPronouns),-wPronouns}", Color.White, false);
        ConsoleHelper.DrawText(" | ", Color.Gray, false);
        ConsoleHelper.DrawText($"{ConsoleHelper.Truncate("FECHA REG.", wDate),-wDate}", Color.Green, false);
        ConsoleHelper.DrawText(" | ", Color.Gray, false);
        ConsoleHelper.DrawText($"{ConsoleHelper.Truncate("RACHA", wStreak),-wStreak}", Color.White);

        int totalWidth = wId + wName + wFandom + wAge + wRoles + wAddRoles + wPronouns + wDate + wStreak + 24;
        ConsoleHelper.DrawText(new string('=', totalWidth), Color.Gray);

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
                    string idCol = (line == 0) ? ConsoleHelper.Truncate(readedUser.userId, wId) : "";
                    string nameCol = (line == 0) ? ConsoleHelper.Truncate(readedUser.name, wName) : "";
                    string fandomCol = (line == 0) ? ConsoleHelper.Truncate(readedUser.fandom, wFandom) : "";
                    string ageCol = (line == 0) ? readedUser.age.ToString() : "";
                    string dateCol = (line == 0) ? readedUser.dateRegistered.ToString() : "";
                    string streakCol = (line == 0) ? readedUser.streak.ToString() : "";

                    string roleCol = (line < roles.Length) ? ConsoleHelper.Truncate(roles[line], wRoles) : "";
                    string addRoleCol = (line < addRoles.Length) ? ConsoleHelper.Truncate(addRoles[line], wAddRoles) : "";
                    string pronounCol = (line < pronounsList.Length) ? ConsoleHelper.Truncate(pronounsList[line], wPronouns) : "";

                    ConsoleHelper.DrawText($"{idCol,-wId}", Color.White, false);
                    ConsoleHelper.DrawText(" | ", Color.Gray, false);
                    ConsoleHelper.DrawText($"{nameCol,-wName}", Color.Green, false);
                    ConsoleHelper.DrawText(" | ", Color.Gray, false);
                    ConsoleHelper.DrawText($"{fandomCol,-wFandom}", Color.White, false);
                    ConsoleHelper.DrawText(" | ", Color.Gray, false);
                    ConsoleHelper.DrawText($"{ageCol,-wAge}", Color.Green, false);
                    ConsoleHelper.DrawText(" | ", Color.Gray, false);
                    ConsoleHelper.DrawText($"{roleCol,-wRoles}", Color.White, false);
                    ConsoleHelper.DrawText(" | ", Color.Gray, false);
                    ConsoleHelper.DrawText($"{addRoleCol,-wAddRoles}", Color.Green, false);
                    ConsoleHelper.DrawText(" | ", Color.Gray, false);
                    ConsoleHelper.DrawText($"{pronounCol,-wPronouns}", Color.White, false);
                    ConsoleHelper.DrawText(" | ", Color.Gray, false);
                    ConsoleHelper.DrawText($"{dateCol,-wDate}", Color.Green, false);
                    ConsoleHelper.DrawText(" | ", Color.Gray, false);
                    ConsoleHelper.DrawText($"{streakCol,-wStreak}", Color.White);
                }

                ConsoleHelper.DrawText(new string('-', totalWidth), Color.DarkGray);
            }
            catch (JsonException)
            {
                ConsoleHelper.DrawText($"[Archivo de usuario corrupto: {Path.GetFileName(jsonFile)}]", Color.Red);
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
        string[] inpa = [""];
        int inpi = 0;
        bool warning = false;

        // --- NOMBRE ---
        while (string.IsNullOrWhiteSpace(inp))
        {
            if (warning) ConsoleHelper.DrawText("No puedes dejar el campo vacío!", Color.Red);
            inp = ConsoleHelper.TakeInput("Ingresa el nombre: ", Color.Yellow);
            inp = string.Concat(inp.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
            if (string.IsNullOrWhiteSpace(inp)) warning = true;
        }

        newUser.name = inp;
        warning = false;
        inp = "";

        // --- ROLES ADICIONALES ---
        inpa = ConsoleHelper.TakeInput("Ingresa roles adicionales (separados por comas, puede estar vacío): ", Color.Yellow)
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0])))
            inpa = ["Ninguno"];

        newUser.additionalRoles = inpa;

        // --- FANDOM ---
        while (string.IsNullOrWhiteSpace(inp))
        {
            if (warning) ConsoleHelper.DrawText("No puedes dejar el campo vacío!", Color.Red);
            inp = ConsoleHelper.TakeInput("Ingresa el fandom: ", Color.Yellow);
            if (string.IsNullOrWhiteSpace(inp)) warning = true;
        }

        newUser.fandom = inp;
        warning = false;
        inp = "";

        // --- ROLES BUSCADOS ---
        inpa = ConsoleHelper.TakeInput("Ingresa los roles buscados (separados por comas, puede estar vacío): ", Color.Yellow)
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0])))
            inpa = ["Ninguno"];

        newUser.lookedCharacters = inpa;

        // --- EDAD ---
        while (string.IsNullOrWhiteSpace(inp) || inpi == 0)
        {
            if (warning) ConsoleHelper.DrawText("Esa edad no es valida!", Color.Red);
            inp = ConsoleHelper.TakeInput("Ingresa la edad: ", Color.Yellow);
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
            if (warning) ConsoleHelper.DrawText("No puedes dejar el campo vacío!", Color.Red);
            inpa = ConsoleHelper.TakeInput("Ingresa los pronombres (separados por comas): ", Color.Yellow)
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0]))) warning = true;
        }

        newUser.pronouns = inpa;

        // --- RACHA ---
        string inputStreak = ConsoleHelper.TakeInput("Ingresa la racha (de tenerla): ", Color.Yellow) ?? "0";
        if (!string.IsNullOrWhiteSpace(inputStreak))
            int.TryParse(inputStreak, out inpi);
        else
            inpi = 0;

        newUser.streak = inpi;

        // --- USER ID ---
        string inputId = ConsoleHelper.TakeInput("Ingresa el ID (Deja vacío para autogenerar): ", Color.Yellow);
        if (string.IsNullOrWhiteSpace(inputId))
            inputId = Guid.NewGuid().ToString();

        newUser.userId = inputId;
        newUser.dateRegistered = DateOnly.FromDateTime(DateTime.Now);

        // Guardar usuario vía Servicio
        UserService.SaveUser(newUser);
        ConsoleHelper.DrawText("Usuario agregado exitosamente!", Color.Green);
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
            ConsoleHelper.DrawText("No hay usuarios registrados para verificar.", Color.Red);
            return;
        }

        ConsoleHelper.DrawText("SELECCIONA UN USUARIO:", Color.White);
        ConsoleHelper.DrawText("");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] userData = lines[i].Split(',');
            ConsoleHelper.DrawText($"{i + 1}. {userData[0]}", Color.Yellow);
        }

        ConsoleHelper.DrawText("");
        string inputSelection = ConsoleHelper.TakeInput("Ingresa el numero del usuario: ");

        if (!int.TryParse(inputSelection, out int selectedIndex) || selectedIndex < 1 || selectedIndex > lines.Length)
        {
            ConsoleHelper.DrawText("Seleccion invalida.", Color.Red);
            return;
        }

        string[] selectedUserData = lines[selectedIndex - 1].Split(',');
        string jsonPath = selectedUserData[1];

        User? currentUser = UserService.LoadUserFromJson(jsonPath);

        if (currentUser == null)
        {
            ConsoleHelper.DrawText($"No se pudo cargar la información del usuario en ({jsonPath}).", Color.Red);
            return;
        }

        ConsoleHelper.DrawText($"Usuario seleccionado: {currentUser.name}", Color.White);
        ConsoleHelper.DrawText("");
        ConsoleHelper.DrawText($"Racha actual: {currentUser.streak}", Color.Gray);
        ConsoleHelper.DrawText("");

        string respuesta = ConsoleHelper.TakeInput("Realizo la actividad de hoy? (s/n): ", Color.Yellow).ToLower();

        if (respuesta == "s")
        {
            currentUser.streak += 1;
            ConsoleHelper.DrawText($"Racha incrementada! Nueva racha: {currentUser.streak}", Color.Green);
        }
        else
        {
            currentUser.streak = 0;
            ConsoleHelper.DrawText("Racha reiniciada a 0.", Color.Red);
        }

        UserService.UpdateUserJson(jsonPath, currentUser);
    }

    public static void Main()
    {
        UserService.EnsureDirectoryExists();
        Console.Clear();

        ConsoleHelper.DrawText(" _   _ ____  _____ ____    ____    _  _____  _    ____    _    ____  _____ ", Color.Yellow);
        ConsoleHelper.DrawText("| | | / ___|| ____|  _ \\  |  _ \\  / \\|_   _|/ \\  | __ )  / \\  / ___|| ____|", Color.Yellow);
        ConsoleHelper.DrawText("| | | \\___ \\|  _| | |_) | | | | |/ _ \\ | | / _ \\ |  _ \\ / _ \\ \\___ \\|  _|  ", Color.DarkYellow);
        ConsoleHelper.DrawText("| |_| |___) | |___|  _ <  | |_| / ___ \\| |/ ___ \\| |_) / ___ \\ ___) | |___", Color.DarkRed);
        ConsoleHelper.DrawText(" \\___/|____/|_____|_| \\_\\ |____/_/   \\_\\_/_/   \\_\\____/_/   \\_\\____/|_____|", Color.Red);
        ConsoleHelper.DrawText("");
        ConsoleHelper.DrawText("UserDB v1.4 (STABLE) - Copyright (c) 2026 CMDPlayer216", Color.Gray);

        while (true)
        {
            ConsoleHelper.DrawText("");
            ConsoleHelper.DrawText("1. Mostrar usuarios");
            ConsoleHelper.DrawText("2. Agregar un usuario");
            ConsoleHelper.DrawText("3. Verificar un usuario");
            ConsoleHelper.DrawText("4. Salir");
            ConsoleHelper.DrawText("");

            string input = ConsoleHelper.TakeInput();
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
                        return;
                    default:
                        ConsoleHelper.DrawText("Esa opcion no existe!", Color.Red);
                        break;
                }
            }
            catch (Exception e)
            {
                ConsoleHelper.DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
            }
        }
    }
}