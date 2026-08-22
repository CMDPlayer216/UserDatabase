using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus;

public static class VerifyUserStreak
{
    public static void Show()
    {
        string logTitle = "VerifyUserStreak";
        Logs.Log(logTitle, "Mostrando menú para verificar racha de usuario", Logs.logType.Info, 2);
        Console.Clear();

        List<string> lines = UserService.GetUserIndexLines();

        if (lines.Count == 0)
        {
            Logs.Log(logTitle, "No hay usuarios registrados para verificar racha", Logs.logType.Info, 1);
            DrawText("No hay usuarios registrados para verificar.", Color.Red);
            return;
        }

        DrawText("SELECCIONA UN USUARIO:", Color.White);
        DrawText("");

        for (int i = 0; i < lines.Count; i++)
        {
            List<string> userData = lines[i].Split(',').ToList();
            DrawText($"{i + 1}. {userData[0]}", Color.Yellow);
        }

        DrawText("");
        string inputSelection = TakeInput("Ingresa el numero del usuario: ");

        if (!int.TryParse(inputSelection, out int selectedIndex) || selectedIndex < 1 || selectedIndex > lines.Count)
        {
            Logs.Log(logTitle, $"Selección de usuario inválida: '{inputSelection}'", Logs.logType.Warning, 2);
            DrawText("Seleccion invalida.", Color.Red);
            return;
        }

        List<string> selectedUserData = lines[selectedIndex - 1].Split(',').ToList();
        string jsonPath = selectedUserData[1];

        User? currentUser = UserService.LoadUserFromJson(jsonPath);

        if (currentUser == null)
        {
            Logs.Log(logTitle, $"Error cargando usuario desde {jsonPath}", Logs.logType.Error, 3);
            DrawText($"No se pudo cargar la información del usuario en ({jsonPath}).", Color.Red);
            return;
        }

        DrawText($"Usuario seleccionado: {currentUser.name}", Color.White);
        DrawText("");
        DrawText($"Racha actual: {currentUser.streak}", Color.Gray);
        DrawText("");

        string respuesta = TakeInput("Realizo la actividad de hoy? (s/n): ", Color.Yellow).ToLower();

        if (respuesta == "s")
        {
            currentUser.streak += 1;
            Logs.Log(logTitle, $"Racha incrementada para {currentUser.userId} ({currentUser.name}) a {currentUser.streak}", Logs.logType.Info, 2);
            DrawText($"Racha incrementada! Nueva racha: {currentUser.streak}", Color.Green);
        }
        else
        {
            currentUser.streak = 0;
            Logs.Log(logTitle, $"Racha reiniciada a 0 para {currentUser.userId} ({currentUser.name})", Logs.logType.Info, 2);
            DrawText("Racha reiniciada a 0.", Color.Red);
        }

        UserService.UpdateUserJson(jsonPath, currentUser);
    }
}
