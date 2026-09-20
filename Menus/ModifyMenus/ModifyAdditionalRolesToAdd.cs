using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para añadir roles adicionales al usuario.
/// Permite ingresar elementos individualmente en un ciclo continuo o varios separados por comas.
/// Valida cada elemento mediante UserValidators.ValidateRole y previene duplicados.
/// </summary>
public static class ModifyAdditionalRolesToAdd
{
    public static List<string> Run(List<string> currentAdditionalRoles, List<string> currentAdditionalRolesToAdd)
    {
        Logs.Log("ModifyAdditionalRolesToAdd", "Iniciando submenú para añadir roles adicionales", Logs.logType.Info, 1);
        List<string> toAdd = new(currentAdditionalRolesToAdd);

        while (true)
        {
            Console.Clear();
            DrawText("=== AÑADIR ROLES ADICIONALES ===", Color.Yellow);
            DrawText("");

            // Visualizar roles adicionales actuales registrados
            DrawText("Roles adicionales actuales del usuario: ", Color.White, insertNewLine: false);
            if (currentAdditionalRoles.Count == 0)
            {
                DrawText("(ninguno)", Color.Gray);
            }
            else
            {
                DrawText(string.Join(", ", currentAdditionalRoles), Color.DarkYellow);
            }

            // Visualizar roles en cola para añadir
            DrawText("Roles en cola para añadir: ", Color.White, insertNewLine: false);
            if (toAdd.Count == 0)
            {
                DrawText("(ninguno)", Color.Gray);
            }
            else
            {
                DrawText(string.Join(", ", toAdd), Color.Green);
            }

            DrawText("");
            DrawText("Ingresa un rol para añadir (o varios separados por comas).");
            DrawText("Presiona [Enter] sin texto para terminar y regresar.");
            DrawText("");

            string input = TakeInput("Rol(es) a añadir: ", Color.Green);

            // Finalizar si presiona enter sin texto
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                Logs.Log("ModifyAdditionalRolesToAdd", $"Finalizando adición de roles adicionales. Total en cola: {toAdd.Count}", Logs.logType.Info, 1);
                break;
            }

            string[] roles = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            bool anyAdded = false;

            foreach (string rol in roles)
            {
                if (!Validators.UserValidators.ValidateRole(rol))
                {
                    DrawText($"El rol \"{rol}\" no es válido.", Color.Red);
                    Logs.Log("ModifyAdditionalRolesToAdd", $"Intento de rol inválido: '{rol}'", Logs.logType.Warning, 2);
                    continue;
                }

                if (currentAdditionalRoles.Contains(rol, StringComparer.OrdinalIgnoreCase) ||
                    toAdd.Contains(rol, StringComparer.OrdinalIgnoreCase))
                {
                    DrawText($"El rol \"{rol}\" ya está presente en el usuario o en la cola para añadir.", Color.DarkYellow);
                    continue;
                }

                toAdd.Add(rol);
                anyAdded = true;
                Logs.Log("ModifyAdditionalRolesToAdd", $"Rol adicional añadido a la cola: '{rol}'", Logs.logType.Info, 1);
                DrawText($"Rol \"{rol}\" añadido a la lista.", Color.Green);
            }

            if (anyAdded)
            {
                DrawText("");
                DrawText("Elemento(s) procesado(s). Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
                Pause();
            }
            else
            {
                DrawText("");
                DrawText("Presiona [Enter] o cualquier tecla para reintentar...", Color.Gray);
                Pause();
            }
        }

        return toAdd;
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

