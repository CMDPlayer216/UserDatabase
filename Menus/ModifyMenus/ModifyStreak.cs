using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para la modificación del campo Racha.
/// Valida la entrada mediante UserValidators.ValidateStreak, que maneja el tipo bool?
/// y el valor neutral -1.
/// </summary>
public static class ModifyStreak
{
    public static int? Run(int currentStreak)
    {
        Logs.Log("ModifyStreak", $"Iniciando modificación de racha. Actual: {currentStreak}", Logs.logType.Info, 1);
        Console.Clear();

        while (true)
        {
            DrawText("=== MODIFICAR RACHA ===", Color.Yellow);
            DrawText($"Racha actual: {currentStreak}", Color.White);
            DrawText("");
            string inputStreak = TakeInput("Nueva racha (enter para cancelar): ", Color.Green);

            // Si se deja en blanco, se cancela la edición y se retorna null
            if (string.IsNullOrEmpty(inputStreak) || string.IsNullOrWhiteSpace(inputStreak))
            {
                Logs.Log("ModifyStreak", "Modificación de racha cancelada por el usuario", Logs.logType.Info, 1);
                return null;
            }

            // Debe ser convertible a número entero
            if (!int.TryParse(inputStreak, out int streak))
            {
                Logs.Log("ModifyStreak", $"Entrada no numérica para racha: '{inputStreak}'", Logs.logType.Warning, 2);
                DrawText("Esa racha no es válida! Debe ingresar un número entero.", Color.Red);
                DrawText("");
                continue;
            }

            // Validar mediante UserValidators.ValidateStreak.
            // Retorna null si es neutral (-1), true si >= 0, false si es menor a 0.
            // Por ende, la condición idiomática del proyecto es 'is not true'.
            if (Validators.UserValidators.ValidateStreak(streak) is not true)
            {
                Logs.Log("ModifyStreak", $"Racha fuera de rango o valor neutral inválido: {streak}", Logs.logType.Warning, 2);
                DrawText("Esa racha no es válida! Debe ser un número entero mayor o igual a 0.", Color.Red);
                DrawText("");
                continue;
            }

            Logs.Log("ModifyStreak", $"Nueva racha aceptada: {streak}", Logs.logType.Info, 1);
            return streak;
        }
    }
}
