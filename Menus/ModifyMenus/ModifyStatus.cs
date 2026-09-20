using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para la modificación del campo Status.
/// Valida la entrada mediante UserValidators.ValidateStatus y despliega la lista
/// de valores soportados en caso de error.
/// </summary>
public static class ModifyStatus
{
    public static string? Run(string currentStatus)
    {
        Logs.Log("ModifyStatus", $"Iniciando modificación de status. Actual: '{currentStatus}'", Logs.logType.Info, 1);
        Console.Clear();

        while (true)
        {
            DrawText("=== MODIFICAR STATUS ===", Color.Yellow);
            DrawText($"Status actual: {currentStatus}", Color.White);
            DrawText("Valores recomendados: Activo, Inactivo, Desconocido, Baneado, Expulsado, Silenciado", Color.Gray);
            DrawText("");
            string status = TakeInput("Nuevo status (enter para cancelar): ", Color.Green);

            // Si se deja vacío, cancela la edición y retorna null
            if (string.IsNullOrEmpty(status) || string.IsNullOrWhiteSpace(status))
            {
                Logs.Log("ModifyStatus", "Modificación de status cancelada por el usuario", Logs.logType.Info, 1);
                return null;
            }

            // Validar mediante UserValidators.ValidateStatus
            if (!Validators.UserValidators.ValidateStatus(status))
            {
                Logs.Log("ModifyStatus", $"Intento de status inválido: '{status}'", Logs.logType.Warning, 2);
                DrawText("Ese status no es válido!", Color.Red);
                DrawText("Status soportados:");
                string[] validStatus = {
                    "Active", "Activo", "Inactive", "Inactivo",
                    "Unknow", "Desconocido", "Banned", "Baneado",
                    "Kicked", "Expulsado", "Silenced", "Silenciado"
                };
                foreach (string cstatus in validStatus)
                {
                    DrawText($"  - {cstatus}", Color.DarkYellow);
                }
                DrawText("");
                continue;
            }

            Logs.Log("ModifyStatus", $"Nuevo status aceptado: '{status}'", Logs.logType.Info, 1);
            return status;
        }
    }
}
