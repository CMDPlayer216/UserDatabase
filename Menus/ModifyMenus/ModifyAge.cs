using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus.ModifyMenus;

/// <summary>
/// Submenú para la modificación del campo Edad.
/// Valida la entrada mediante UserValidators.ValidateAge, respetando el tipo de retorno bool?
/// y el valor neutral -1.
/// </summary>
public static class ModifyAge
{
    public static int? Run(int currentAge)
    {
        Logs.Log("ModifyAge", $"Iniciando modificación de edad. Actual: {currentAge}", Logs.logType.Info, 1);
        Console.Clear();

        while (true)
        {
            DrawText("=== MODIFICAR EDAD ===", Color.Yellow);
            DrawText($"Edad actual: {currentAge}", Color.White);
            DrawText("");
            string inputAge = TakeInput("Nueva edad (enter para cancelar): ", Color.Green);

            // Si se presiona enter sin datos, cancelar la modificación
            if (string.IsNullOrEmpty(inputAge) || string.IsNullOrWhiteSpace(inputAge))
            {
                Logs.Log("ModifyAge", "Modificación de edad cancelada por el usuario", Logs.logType.Info, 1);
                return null;
            }

            // Debe poder parsearse como entero
            if (!int.TryParse(inputAge, out int age))
            {
                Logs.Log("ModifyAge", $"Entrada no numérica para edad: '{inputAge}'", Logs.logType.Warning, 2);
                DrawText("Esa edad no es válida! Debe ingresar un número entero.", Color.Red);
                DrawText("");
                continue;
            }

            // Validar mediante UserValidators.ValidateAge. Retorna bool?: null si es neutral (-1), true si está en rango, false si no.
            if (Validators.UserValidators.ValidateAge(age) is not true)
            {
                Logs.Log("ModifyAge", $"Edad fuera de rango o neutral inválido: {age}", Logs.logType.Warning, 2);
                DrawText("Esa edad no es válida! Debe ser un número mayor o igual a 0.", Color.Red);
                DrawText("");
                continue;
            }

            Logs.Log("ModifyAge", $"Nueva edad aceptada: {age}", Logs.logType.Info, 1);
            return age;
        }
    }
}

