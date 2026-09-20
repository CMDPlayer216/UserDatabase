using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para eliminar roles buscados del usuario.
/// Muestra los roles disponibles numerados y permite al usuario eliminarlos individualmente
/// especificando el número de opción o el nombre del rol. Si ya está seleccionado,
/// permite desmarcarlo (comportamiento tipo toggle).
/// </summary>
public static class ModifyWantedRolesToRemove
{
    public static List<string> Run(List<string> currentWantedRoles, List<string> currentWantedRolesToRemove)
    {
        Logs.Log("ModifyWantedRolesToRemove", "Iniciando submenú para eliminar roles buscados", Logs.logType.Info, 1);
        List<string> toRemove = new(currentWantedRolesToRemove);

        // Si el usuario no tiene roles registrados, no hay nada que eliminar
        if (currentWantedRoles.Count == 0)
        {
            Console.Clear();
            DrawText("=== ELIMINAR ROLES BUSCADOS ===", Color.Yellow);
            DrawText("");
            DrawText("El usuario no tiene roles buscados registrados para eliminar.", Color.DarkYellow);
            DrawText("");
            DrawText("Presiona [Enter] o cualquier tecla para volver...", Color.Gray);
            Pause();
            return toRemove;
        }

        while (true)
        {
            Console.Clear();
            DrawText("=== ELIMINAR ROLES BUSCADOS ===", Color.Yellow);
            DrawText("");
            DrawText("Roles registrados en el usuario:");

            // Listar cada rol con su índice numérico para facilitar la selección
            for (int i = 0; i < currentWantedRoles.Count; i++)
            {
                string rol = currentWantedRoles[i];
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

            // Mostrar el resumen de los que ya están encolados para borrado
            if (toRemove.Count > 0)
            {
                DrawText("");
                DrawText("Roles en cola para eliminar: ", Color.White, insertNewLine: false);
                DrawText(string.Join(", ", toRemove), Color.Red);
            }

            DrawText("");
            DrawText("Ingresa el número o nombre del rol que deseas eliminar individualmente.");
            DrawText("Si seleccionas uno ya marcado, se desmarcará.");
            DrawText("Presiona [Enter] sin texto para terminar y regresar.");
            DrawText("");

            string input = TakeInput("Rol a eliminar / desmarcar: ", Color.Green);

            // Salir al presionar enter vacío
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                Logs.Log("ModifyWantedRolesToRemove", $"Finalizando eliminación de roles buscados. Total marcados: {toRemove.Count}", Logs.logType.Info, 1);
                break;
            }

            string? targetRole = null;

            // Intentar selección por índice numérico (1-based)
            if (int.TryParse(input, out int index) && index >= 1 && index <= currentWantedRoles.Count)
            {
                targetRole = currentWantedRoles[index - 1];
            }
            else
            {
                // Intentar selección por coincidencia de texto
                targetRole = currentWantedRoles.FirstOrDefault(r => string.Equals(r, input.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (targetRole == null)
            {
                DrawText($"No se encontró ningún rol correspondiente a \"{input}\".", Color.Red);
                DrawText("Presiona [Enter] o cualquier tecla para reintentar...", Color.Gray);
                Pause();
                continue;
            }

            // Si ya estaba en la lista de eliminaciones, se desmarca (toggle)
            if (toRemove.Contains(targetRole, StringComparer.OrdinalIgnoreCase))
            {
                toRemove.RemoveAll(r => string.Equals(r, targetRole, StringComparison.OrdinalIgnoreCase));
                Logs.Log("ModifyWantedRolesToRemove", $"Rol desmarcado de eliminación: '{targetRole}'", Logs.logType.Info, 1);
                DrawText($"Rol \"{targetRole}\" desmarcado de eliminación.", Color.Yellow);
            }
            else
            {
                toRemove.Add(targetRole);
                Logs.Log("ModifyWantedRolesToRemove", $"Rol marcado para eliminar individualmente: '{targetRole}'", Logs.logType.Info, 1);
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