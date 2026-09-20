using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para eliminar pronombres del usuario.
/// Muestra los pronombres disponibles numerados y permite al usuario eliminarlos
/// individualmente por número o texto del pronombre, o desmarcarlos (toggle).
/// </summary>
public static class ModifyPronounsToRemove
{
    public static List<string> Run(List<string> currentPronouns, List<string> currentPronounsToRemove, List<string>? currentPronounsToAdd = null)
    {
        Logs.Log("ModifyPronounsToRemove", "Iniciando submenú para eliminar pronombres", Logs.logType.Info, 1);
        List<string> toRemove = new(currentPronounsToRemove);

        if (currentPronouns.Count == 0)
        {
            Console.Clear();
            DrawText("=== ELIMINAR PRONOMBRES ===", Color.Yellow);
            DrawText("");
            DrawText("El usuario no tiene pronombres registrados para eliminar.", Color.DarkYellow);
            DrawText("");
            DrawText("Presiona [Enter] o cualquier tecla para volver...", Color.Gray);
            Pause();
            return toRemove;
        }

        while (true)
        {
            Console.Clear();
            DrawText("=== ELIMINAR PRONOMBRES ===", Color.Yellow);
            DrawText("");
            DrawText("Pronombres registrados en el usuario:");

            for (int i = 0; i < currentPronouns.Count; i++)
            {
                string pronoun = currentPronouns[i];
                bool isMarked = toRemove.Contains(pronoun, StringComparer.OrdinalIgnoreCase);

                if (isMarked)
                {
                    DrawText($"  [{i + 1}] {pronoun} (Marcado para eliminar)", Color.DarkRed);
                }
                else
                {
                    DrawText($"  [{i + 1}] {pronoun}", Color.White);
                }
            }

            if (toRemove.Count > 0)
            {
                DrawText("");
                DrawText("Pronombres en cola para eliminar: ", Color.White, insertNewLine: false);
                DrawText(string.Join(", ", toRemove), Color.Red);
            }

            DrawText("");
            DrawText("Ingresa el número o pronombre que deseas eliminar individualmente.");
            DrawText("Si seleccionas uno ya marcado, se desmarcará.");
            DrawText("Presiona [Enter] sin texto para terminar y regresar.");
            DrawText("");

            string input = TakeInput("Pronombre a eliminar / desmarcar: ", Color.Green);

            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                Logs.Log("ModifyPronounsToRemove", $"Finalizando eliminación de pronombres. Total marcados: {toRemove.Count}", Logs.logType.Info, 1);
                break;
            }

            string? targetPronoun = null;

            if (int.TryParse(input, out int index) && index >= 1 && index <= currentPronouns.Count)
            {
                targetPronoun = currentPronouns[index - 1];
            }
            else
            {
                targetPronoun = currentPronouns.FirstOrDefault(r => string.Equals(r, input.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (targetPronoun == null)
            {
                DrawText($"No se encontró ningún pronombre correspondiente a \"{input}\".", Color.Red);
                DrawText("Presiona [Enter] o cualquier tecla para reintentar...", Color.Gray);
                Pause();
                continue;
            }

            if (toRemove.Contains(targetPronoun, StringComparer.OrdinalIgnoreCase))
            {
                toRemove.RemoveAll(r => string.Equals(r, targetPronoun, StringComparison.OrdinalIgnoreCase));
                Logs.Log("ModifyPronounsToRemove", $"Pronombre desmarcado de eliminación: '{targetPronoun}'", Logs.logType.Info, 1);
                DrawText($"Pronombre \"{targetPronoun}\" desmarcado de eliminación.", Color.Yellow);
            }
            else
            {
                // Validar que no se eliminen todos los pronombres (campo obligatorio)
                int addedCount = currentPronounsToAdd?.Count ?? 0;
                int remainingCurrent = currentPronouns.Count(p => !toRemove.Contains(p, StringComparer.OrdinalIgnoreCase) && !string.Equals(p, targetPronoun, StringComparison.OrdinalIgnoreCase));
                int totalRemaining = remainingCurrent + addedCount;

                if (totalRemaining <= 0)
                {
                    Logs.Log("ModifyPronounsToRemove", $"Intento de eliminar todos los pronombres bloqueado para '{targetPronoun}'", Logs.logType.Warning, 2);
                    DrawText("");
                    DrawText("No puedes eliminar todos los pronombres! El campo de pronombres no puede quedar vacío.", Color.Red);
                    DrawText("Si deseas cambiar tus pronombres, primero añade los nuevos en 'Añadir pronombres'.", Color.Yellow);
                    DrawText("Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
                    Pause();
                    continue;
                }

                toRemove.Add(targetPronoun);
                Logs.Log("ModifyPronounsToRemove", $"Pronombre marcado para eliminar: '{targetPronoun}'", Logs.logType.Info, 1);
                DrawText($"Pronombre \"{targetPronoun}\" marcado para eliminar.", Color.Green);
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
