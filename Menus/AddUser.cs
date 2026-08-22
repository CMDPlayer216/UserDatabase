using NanoidDotNet;
using userdb.Commands;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus;

public static class AddUserMenu
{
    public static void Show()
    {
        Logs.Log("AddUserMenu", "Mostrando menú interactivo para agregar usuario", Logs.logType.Info, 2);
        Console.Clear();

        string name;
        List<string> additionalRoles;
        string fandom;
        List<string> lookedCharacters;
        int age;
        List<string> pronouns;
        int streak;
        string userId;

        string inp = "";
        List<string> inpa = Array.Empty<string>().ToList();
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

        name = inp;
        warning = false;
        inp = "";

        // --- ROLES ADICIONALES ---
        inpa = TakeInput("Ingresa roles adicionales (separados por comas, puede estar vacío): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

        if (inpa == null || inpa.Count == 0 || (inpa.Count == 1 && string.IsNullOrWhiteSpace(inpa[0])))
            inpa = Array.Empty<string>().ToList();

        additionalRoles = inpa;

        // --- FANDOM ---
        while (string.IsNullOrWhiteSpace(inp))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inp = TakeInput("Ingresa el fandom: ", Color.Yellow);
            if (string.IsNullOrWhiteSpace(inp)) warning = true;
        }

        fandom = inp;
        warning = false;
        inp = "";

        // --- ROLES BUSCADOS ---
        inpa = TakeInput("Ingresa los roles buscados (separados por comas, puede estar vacío): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

        if (inpa == null || inpa.Count == 0 || (inpa.Count == 1 && string.IsNullOrWhiteSpace(inpa[0])))
            inpa = Array.Empty<string>().ToList();

        lookedCharacters = inpa;

        // --- EDAD ---
        while (string.IsNullOrWhiteSpace(inp) || inpi == 0)
        {
            if (warning) DrawText("Esa edad no es valida!", Color.Red);
            inp = TakeInput("Ingresa la edad: ", Color.Yellow);
            if (string.IsNullOrWhiteSpace(inp)) warning = true;
            if (!string.IsNullOrWhiteSpace(inp)) int.TryParse(inp, out inpi);
        }

        age = inpi;
        warning = false;

        // --- PRONOMBRES ---
        inpa = Array.Empty<string>().ToList();
        while (inpa == null || inpa.Count == 0 || (inpa.Count == 1 && string.IsNullOrWhiteSpace(inpa[0])))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inpa = TakeInput("Ingresa los pronombres (separados por comas): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            if (inpa == null || inpa.Count == 0 || (inpa.Count == 1 && string.IsNullOrWhiteSpace(inpa[0]))) warning = true;
        }

        pronouns = inpa;

        // --- RACHA ---
        string inputStreak = TakeInput("Ingresa la racha (de tenerla): ", Color.Yellow) ?? "0";
        if (!string.IsNullOrWhiteSpace(inputStreak))
            int.TryParse(inputStreak, out inpi);
        else
            inpi = 0;

        streak = inpi;

        // --- USER ID ---
        string inputId = TakeInput("Ingresa el ID (Deja vacío para autogenerar): ", Color.Yellow);
        inputId = string.Concat(inputId.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
        if (string.IsNullOrWhiteSpace(inputId))
            inputId = Nanoid.Generate(size: 12);

        userId = inputId;

        inp = TakeInput("Ingresa el status (Deja vacío para establecer en activo): ", Color.Yellow);
        inp = string.Concat(inp.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
        if (string.IsNullOrWhiteSpace(inp))
            inp = "Activo";
        string status = inp;
        
        // Guardar usuario vía Servicio
        AddUser.Run(name, additionalRoles, fandom, lookedCharacters, age, pronouns, streak, userId, status);
    }
}
