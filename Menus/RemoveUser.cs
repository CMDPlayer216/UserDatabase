using userdb.Commands;
using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus;

public static class RemoveUserMenu
{
    public static void Show()
    {
        string logTitle = "RemoveUserMenu";
        Logs.Log(logTitle, "Mostrando menú interactivo para eliminar usuario", Logs.logType.Info, 2);
        Console.Clear();

        List<string> lines = UserService.GetUserIndexLines();

        if (lines.Count == 0)
        {
            Logs.Log(logTitle, "No hay usuarios registrados para eliminar", Logs.logType.Info, 1);
            DrawText("No hay usuarios registrados para eliminar.", Color.Red);
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

        User? user = UserService.LoadUserFromJson(selectedUserData[1]);

        if (user == null)
        {
            Logs.Log(logTitle, $"Error al cargar usuario para eliminar: {selectedUserData[1]}", Logs.logType.Error, 3);
            DrawText($"Error cargando el archivo {selectedUserData[1]}", Color.Red);
            return;
        }

        Logs.Log(logTitle, $"Usuario seleccionado para eliminar: {user.userId} ({user.name})", Logs.logType.Info, 2);
        DeleteUser.Run(user.userId);
    }
}
