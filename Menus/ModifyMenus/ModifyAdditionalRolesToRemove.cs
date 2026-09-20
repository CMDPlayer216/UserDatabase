using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para eliminar roles adicionales del usuario.
/// Muestra los roles adicionales disponibles numerados y permite al usuario eliminarlos
/// individualmente por número o nombre de rol, o desmarcarlos (toggle).
/// </summary>
public static class ModifyAdditionalRolesToRemove
{
    public static List<string> Run(List<string> currentAdditionalRoles, List<string> currentAdditionalRolesToRemove)
    {
        Logs.Log("ModifyAdditionalRolesToRemove", "Iniciando submenú para eliminar roles adicionales", Logs.logType.Info, 1);
        List<string> toRemove = new(currentAdditionalRolesToRemove);

        if (currentAdditionalRoles.Count == 0)
        {
            Console.Clear();
            DrawText("=== ELIMINAR ROLES ADICIONALES ===", Color.Yellow);
            DrawText("");
            DrawText("El usuario no tiene roles adicionales registrados para eliminar.", Color.DarkYellow);
            DrawText("");
            DrawText("Presiona [Enter] o cualquier tecla para volver...", Color.Gray);
            Pause();
            return toRemove;
        }

        while (true)
        {
            Console.Clear();
            DrawText("=== ELIMINAR ROLES ADICIONALES ===", Color.Yellow);
            DrawText("");
            DrawText("Roles adicionales registrados en el usuario:");

            for (int i = 0; i < currentAdditionalRoles.Count; i++)
            {
                string rol = currentAdditionalRoles[i];
                bool isMarked = toRemove.Contains(rol, StringComparer.OrdinalIgnoreCase);

                if (isMarked)
                {
                    DrawText($"  [{i + 1}] {rol} (Marcado para eliminar)", Color.DarkRed);
                }
                else
                {
                    DrawText($"  [{i + 1}] {rol}", Color.White);
                }
            }

            if (toRemove.Count > 0)
            {
                DrawText("");
                DrawText("Roles adicionales en cola para eliminar: ", Color.White, insertNewLine: false);
                DrawText(string.Join(", ", toRemove), Color.Red);
            }

            DrawText("");
            DrawText("Ingresa el número o nombre del rol adicional que deseas eliminar individualmente.");
            DrawText("Si seleccionas uno ya marcado, se desmarcará.");
            DrawText("Presiona [Enter] sin texto para terminar y regresar.");
            DrawText("");

            string input = TakeInput("Rol a eliminar / desmarcar: ", Color.Green);

            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                Logs.Log("ModifyAdditionalRolesToRemove", $"Finalizando eliminación de roles adicionales. Total marcados: {toRemove.Count}", Logs.logType.Info, 1);
                break;
            }

            string? targetRole = null;

            if (int.TryParse(input, out int index) && index >= 1 && index <= currentAdditionalRoles.Count)
            {
                targetRole = currentAdditionalRoles[index - 1];
            }
            else
            {
                targetRole = currentAdditionalRoles.FirstOrDefault(r => string.Equals(r, input.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (targetRole == null)
            {
                DrawText($"No se encontró ningún rol correspondiente a \"{input}\".", Color.Red);
                DrawText("Presiona [Enter] o cualquier tecla para reintentar...", Color.Gray);
                Pause();
                continue;
            }

            if (toRemove.Contains(targetRole, StringComparer.OrdinalIgnoreCase))
            {
                toRemove.RemoveAll(r => string.Equals(r, targetRole, StringComparison.OrdinalIgnoreCase));
                Logs.Log("ModifyAdditionalRolesToRemove", $"Rol desmarcado de eliminación: '{targetRole}'", Logs.logType.Info, 1);
                DrawText($"Rol \"{targetRole}\" desmarcado de eliminación.", Color.Yellow);
            }
            else
            {
                toRemove.Add(targetRole);
                Logs.Log("ModifyAdditionalRolesToRemove", $"Rol marcado para eliminar: '{targetRole}'", Logs.logType.Info, 1);
                DrawText($"Rol \"{targetRole}\" marcado para eliminar.", Color.Green);
            }

            DrawText("");
            DrawText("Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
            Pause();
        }

        return toRemove;
    }

    /// <summary>
    /// Pausa la ejecución esperando una tecla, soportando de forma segura entradas estándar redirigidas.
    /// </summary>
    private static void Pause()
    {
        if (Console.IsInputRedirected)
            Console.ReadLine();
        else
            Console.ReadKey(true);
    }
}
