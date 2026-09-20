using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para la modificación del campo Nombre.
/// Valida la entrada mediante UserValidators.ValidateName.
/// </summary>
public static class ModifyName
{
    public static string? Run(string currentName)
    {
        Logs.Log("ModifyName", $"Iniciando modificación de nombre. Actual: '{currentName}'", Logs.logType.Info, 1);
        Console.Clear();

        while (true)
        {
            DrawText("=== MODIFICAR NOMBRE ===", Color.Yellow);
            DrawText($"Nombre actual: {currentName}", Color.White);
            DrawText("");
            string name = TakeInput("Nuevo nombre (enter para cancelar): ", Color.Green);

            // Si se deja vacío o solo con espacios, se cancela la edición y se retorna null
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                Logs.Log("ModifyName", "Modificación de nombre cancelada por el usuario", Logs.logType.Info, 1);
                return null;
            }

            // Validación de caracteres permitidos según las reglas del validador de nombres
            if (!Validators.UserValidators.ValidateName(name))
            {
                Logs.Log("ModifyName", $"Intento de nombre no válido: '{name}'", Logs.logType.Warning, 2);
                DrawText("Ese nombre no es válido! Solo se permiten caracteres alfanuméricos y signos comunes.", Color.Red);
                DrawText("");
                continue;
            }

            Logs.Log("ModifyName", $"Nuevo nombre aceptado: '{name}'", Logs.logType.Info, 1);
            return name;
        }
    }
}

