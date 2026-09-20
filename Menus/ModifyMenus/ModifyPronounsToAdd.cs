using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para añadir pronombres al usuario.
/// Permite ingresar pronombres individualmente en un ciclo interactivo o varios separados por comas.
/// Valida cada elemento con UserValidators.ValidatePronoun y previene duplicados.
/// </summary>
public static class ModifyPronounsToAdd
{
    public static List<string> Run(List<string> currentPronouns, List<string> currentPronounsToAdd)
    {
        Logs.Log("ModifyPronounsToAdd", "Iniciando submenú para añadir pronombres", Logs.logType.Info, 1);
        List<string> toAdd = new(currentPronounsToAdd);

        while (true)
        {
            Console.Clear();
            DrawText("=== AÑADIR PRONOMBRES ===", Color.Yellow);
            DrawText("");

            // Visualizar pronombres actuales registrados
            DrawText("Pronombres actuales del usuario: ", Color.White, insertNewLine: false);
            if (currentPronouns.Count == 0)
            {
                DrawText("(ninguno)", Color.Gray);
            }
            else
            {
                DrawText(string.Join(", ", currentPronouns), Color.DarkYellow);
            }

            // Visualizar pronombres en cola para añadir
            DrawText("Pronombres en cola para añadir: ", Color.White, insertNewLine: false);
            if (toAdd.Count == 0)
            {
                DrawText("(ninguno)", Color.Gray);
            }
            else
            {
                DrawText(string.Join(", ", toAdd), Color.Green);
            }

            DrawText("");
            DrawText("Ingresa un pronombre para añadir (o varios separados por comas).");
            DrawText("Presiona [Enter] sin texto para terminar y regresar.");
            DrawText("");

            string input = TakeInput("Pronombre(s) a añadir: ", Color.Green);

            // Finalizar si se presiona Enter vacío
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                Logs.Log("ModifyPronounsToAdd", $"Finalizando adición de pronombres. Total en cola: {toAdd.Count}", Logs.logType.Info, 1);
                break;
            }

            string[] pronouns = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            bool anyAdded = false;

            foreach (string pronoun in pronouns)
            {
                // Validación de pronombres según UserValidators (no permite espacios, sí guiones)
                if (!Validators.UserValidators.ValidatePronoun(pronoun))
                {
                    DrawText($"El pronombre \"{pronoun}\" no es válido.", Color.Red);
                    Logs.Log("ModifyPronounsToAdd", $"Intento de pronombre no válido: '{pronoun}'", Logs.logType.Warning, 2);
                    continue;
                }

                if (currentPronouns.Contains(pronoun, StringComparer.OrdinalIgnoreCase) ||
                    toAdd.Contains(pronoun, StringComparer.OrdinalIgnoreCase))
                {
                    DrawText($"El pronombre \"{pronoun}\" ya está presente en el usuario o en la cola para añadir.", Color.DarkYellow);
                    continue;
                }

                toAdd.Add(pronoun);
                anyAdded = true;
                Logs.Log("ModifyPronounsToAdd", $"Pronombre añadido a la cola: '{pronoun}'", Logs.logType.Info, 1);
                DrawText($"Pronombre \"{pronoun}\" añadido a la lista.", Color.Green);
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

