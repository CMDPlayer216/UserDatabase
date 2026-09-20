using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para la modificación del campo Fandom.
/// Valida la entrada mediante UserValidators.ValidateFandom.
/// </summary>
public static class ModifyFandom
{
    public static string? Run(string currentFandom)
    {
        Logs.Log("ModifyFandom", $"Iniciando modificación de fandom. Actual: '{currentFandom}'", Logs.logType.Info, 1);
        Console.Clear();

        while (true)
        {
            DrawText("=== MODIFICAR FANDOM ===", Color.Yellow);
            DrawText($"Fandom actual: {currentFandom}", Color.White);
            DrawText("");
            string fandom = TakeInput("Nuevo fandom (enter para cancelar): ", Color.Green);

            // Cancelar si se presiona enter sin texto
            if (string.IsNullOrEmpty(fandom) || string.IsNullOrWhiteSpace(fandom))
            {
                Logs.Log("ModifyFandom", "Modificación de fandom cancelada por el usuario", Logs.logType.Info, 1);
                return null;
            }

            // Validar caracteres del fandom
            if (!Validators.UserValidators.ValidateFandom(fandom))
            {
                Logs.Log("ModifyFandom", $"Intento de fandom no válido: '{fandom}'", Logs.logType.Warning, 2);
                DrawText("Ese fandom no es válido! Solo se permiten caracteres alfanuméricos y signos comunes.", Color.Red);
                DrawText("");
                continue;
            }

            Logs.Log("ModifyFandom", $"Nuevo fandom aceptado: '{fandom}'", Logs.logType.Info, 1);
            return fandom;
        }
    }
}

