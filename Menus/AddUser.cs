using userdb.Commands;
using userdb.Services;
using userdb.Validators;
using static NanoidDotNet.Nanoid;
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
        string status;

        string inp = "";
        List<string> inpa = [];
        int inpi = -1;
        bool valid = false;
        bool warning = false;

        bool invalidElement = false;

        // --- NOMBRE ---
        while (!valid)
        {
            if (warning) DrawText("El nombre no es válido o está vacío", Color.Red);
            inp = TakeInput("Ingresa el nombre: ", Color.Yellow);
            valid = UserValidators.ValidateName(inp);
            if (!valid) warning = true;
        }

        name = inp;
        warning = false;
        valid = false;
        inp = "";

        // --- ROLES ADICIONALES ---
        while (!valid)
        {
            invalidElement = false;
            inpa = [.. TakeInput("Ingresa roles adicionales (separados por comas, puede estar vacío): ", Color.Yellow)
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)];

            if (inpa == null || inpa.Count == 0 || (inpa.Count == 1 && string.IsNullOrWhiteSpace(inpa[0]))) { inpa = []; break; }

            else
            {
                foreach (string role in inpa)
                {
                    if (!UserValidators.ValidateRole(role))
                    {
                        DrawText($"El rol \"{role}\" no es válido", Color.Red);
                        invalidElement = true;
                        break;
                    }
                }
                valid = !invalidElement;
            }
        }

        additionalRoles = inpa;
        inpa = [];

        // --- FANDOM ---
        while (!valid)
        {
            if (warning) DrawText("El fandom no es válido o está vacío", Color.Red);
            inp = TakeInput("Ingresa el fandom: ", Color.Yellow);
            valid = UserValidators.ValidateFandom(inp);
            if (!valid) warning = true;
        }

        fandom = inp;
        warning = false;
        valid = false;
        inp = "";

        // --- ROLES BUSCADOS ---
        while (!valid)
        {
            invalidElement = false;
            inpa = [.. TakeInput("Ingresa los roles buscados (separados por comas, puede estar vacío): ", Color.Yellow)
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)];

            if (inpa == null || inpa.Count == 0 || (inpa.Count == 1 && string.IsNullOrWhiteSpace(inpa[0]))) { inpa = []; break; }
            else
            {
                foreach (string role in inpa)
                {
                    if (!UserValidators.ValidateRole(role))
                    {
                        DrawText($"El rol \"{role}\" no es válido", Color.Red);
                        invalidElement = true;
                        break;
                    }
                }
                valid = !invalidElement;
            }
        }

        lookedCharacters = inpa;
        valid = false;
        inpa = [];

        // --- EDAD ---
        while (!valid)
        {
            if (warning) DrawText("Esa edad no es valida!", Color.Red);
            inp = TakeInput("Ingresa la edad: ", Color.Yellow);
            if (!int.TryParse(inp, out inpi))
            {
                warning = true;
                continue;
            }
            if (UserValidators.ValidateAge(inpi) is not true)
            {
                warning = true;
                continue;
            }
            valid = true;
        }

        age = inpi;
        inpi = -1;
        valid = false;
        warning = false;
        inp = "";

        // --- PRONOMBRES ---
        while (!valid)
        {
            invalidElement = false;
            inpa = [.. TakeInput("Ingresa los pronombres (separados por comas): ", Color.Yellow)
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)];
            if (inpa == null || inpa.Count == 0 || (inpa.Count == 1 && string.IsNullOrWhiteSpace(inpa[0])))
            {
                DrawText("No puedes dejar este campo vacío!", Color.Red);
                inpa = [];
                continue;
            }
            foreach (string pronoun in inpa)
            {
                if (!UserValidators.ValidatePronoun(pronoun))
                {
                    DrawText($"El pronombre \"{pronoun}\" es inválido.", Color.Red);
                    invalidElement = true;
                    break;
                }
            }
            valid = !invalidElement;
        }

        valid = false;
        pronouns = inpa;

        // --- RACHA ---
        while (!valid)
        {
            inp = TakeInput("Ingresa la racha (de tenerla): ", Color.Yellow);
            if (string.IsNullOrEmpty(inp))
            {
                inpi = 0;
                break;
            }
            if (!int.TryParse(inp, out inpi) && UserValidators.ValidateStreak(inpi) is not true)
            {
                DrawText("Eso no es una racha válida!", Color.Red);
                continue;
            }
            valid = true;
        }
        streak = inpi;
        inp = "";
        valid = false;

        // --- USER ID ---
        while (!valid)
        {
            inp = TakeInput("Ingresa el ID (Deja vacío para autogenerar): ", Color.Yellow);
            if (string.IsNullOrEmpty(inp))
            {
                inp = Nanoid.Generate(size: 12);
                break;
            }
            if (!UserValidators.ValidateId(inp)) DrawText("Esa ID no es válida!", Color.Red);
        }
        userId = inp;
        inp = "";

        while (!valid)
        {
            inp = TakeInput("Ingresa el status (Deja vacío para establecer en activo): ", Color.Yellow);
            if (string.IsNullOrEmpty(inp))
            {
                inp = "Activo";
                break;
            }
            if (!UserValidators.ValidateStatus(inp))
            {
                DrawText("Ese status no es válido!", Color.Red);
                DrawText("Status soportados:");
                string[] validStatus = {"Active"   , "Activo"  , "Inactive",
                                "Inactivo" , "Unknow"  , "Desconocido",
                                "Banned"   , "Baneado" , "Kicked",
                                "Expulsado", "Silenced", "Silenciado"};
                foreach (string cstatus in validStatus)
                {
                    DrawText($"- {cstatus}");
                }
            }

            else { valid = true; }
        }
        status = inp;

        // Guardar usuario vía Servicio
        AddUser.Run(name, additionalRoles, fandom, lookedCharacters, age, pronouns, streak, userId, status);
    }
}
