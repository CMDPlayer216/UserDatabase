using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para la modificación del campo ID (userId).
/// Valida la entrada mediante UserValidators.ValidateId.
/// Nota: Modificar el ID provocará que al guardar se renombre el archivo .json correspondiente.
/// </summary>
public static class ModifyUserId
{
    public static string? Run(string currentId)
    {
        Logs.Log("ModifyUserId", $"Iniciando modificación de ID. Actual: '{currentId}'", Logs.logType.Info, 1);
        Console.Clear();

        while (true)
        {
            DrawText("=== MODIFICAR ID ===", Color.Yellow);
            DrawText($"ID actual: {currentId}", Color.White);
            DrawText("Nota: Modificar el ID renombrará el archivo JSON del usuario al guardar.", Color.Gray);
            DrawText("");
            string id = TakeInput("Nuevo ID (enter para cancelar): ", Color.Green);

            // Si se deja vacío, cancela la edición y retorna null
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                Logs.Log("ModifyUserId", "Modificación de ID cancelada por el usuario", Logs.logType.Info, 1);
                return null;
            }

            // Validar mediante UserValidators.ValidateId (alfanuméricos, guiones y guiones bajos)
            if (!Validators.UserValidators.ValidateId(id))
            {
                Logs.Log("ModifyUserId", $"Intento de ID no válido: '{id}'", Logs.logType.Warning, 2);
                DrawText("Esa ID no es válida! Solo puede contener letras, números, guiones (-) y guiones bajos (_).", Color.Red);
                DrawText("");
                continue;
            }

            Logs.Log("ModifyUserId", $"Nuevo ID aceptado: '{id}'", Logs.logType.Info, 1);
            return id;
        }
    }
}
