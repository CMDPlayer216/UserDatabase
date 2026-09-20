using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para añadir roles buscados al usuario.
/// Permite ingresar elementos individualmente en un ciclo continuo o varios separados por comas.
/// Valida cada elemento con UserValidators.ValidateRole y previene duplicados.
/// </summary>
public static class ModifyWantedRolesToAdd
{
    public static List<string> Run(List<string> currentWantedRoles, List<string> currentWantedRolesToAdd)
    {
        Logs.Log("ModifyWantedRolesToAdd", "Iniciando submenú para añadir roles buscados", Logs.logType.Info, 1);
        // Creamos una copia de la lista acumulada para no mutar el estado si ocurre algún error
        List<string> toAdd = new(currentWantedRolesToAdd);

        while (true)
        {
            Console.Clear();
            DrawText("=== AÑADIR ROLES BUSCADOS ===", Color.Yellow);
            DrawText("");

            // Visualizar los roles actuales ya registrados en el usuario
            DrawText("Roles buscados actuales del usuario: ", Color.White, insertNewLine: false);
            if (currentWantedRoles.Count == 0)
            {
                DrawText("(ninguno)", Color.Gray);
            }
            else
            {
                DrawText(string.Join(", ", currentWantedRoles), Color.DarkYellow);
            }

            // Visualizar los roles que se encuentran en cola para añadir en esta sesión
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

            // Presionar Enter con entrada vacía indica que se ha terminado de añadir elementos
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                Logs.Log("ModifyWantedRolesToAdd", $"Finalizando adición de roles buscados. Total en cola: {toAdd.Count}", Logs.logType.Info, 1);
                break;
            }

            string[] roles = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            bool anyAdded = false;

            foreach (string rol in roles)
            {
                // Validación de caracteres permitidos para el rol
                if (!Validators.UserValidators.ValidateRole(rol))
                {
                    DrawText($"El rol \"{rol}\" no es válido.", Color.Red);
                    Logs.Log("ModifyWantedRolesToAdd", $"Intento de rol inválido: '{rol}'", Logs.logType.Warning, 2);
                    continue;
                }

                // Verificar si ya existe en el perfil original o en la lista de adición actual
                if (currentWantedRoles.Contains(rol, StringComparer.OrdinalIgnoreCase) ||
                    toAdd.Contains(rol, StringComparer.OrdinalIgnoreCase))
                {
                    DrawText($"El rol \"{rol}\" ya está presente en el usuario o en la cola para añadir.", Color.DarkYellow);
                    continue;
                }

                toAdd.Add(rol);
                anyAdded = true;
                Logs.Log("ModifyWantedRolesToAdd", $"Rol añadido individualmente a la cola: '{rol}'", Logs.logType.Info, 1);
                DrawText($"Rol \"{rol}\" añadido a la lista.", Color.Green);
            }

            // Pequeña pausa estética para que el usuario pueda leer los mensajes si hubo adiciones o advertencias
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