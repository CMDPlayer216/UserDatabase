using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;
using static userdb.Services.UserService;

namespace userdb.Commands;

public static class DeleteUser
{
    public static void Run(string userId, bool noConfirm = false)
    {
        string logTitle = "DeleteUser";
        Logs.Log(logTitle, $"Iniciando eliminación de usuario ID: {userId}, noConfirm: {noConfirm}", Logs.logType.Info, 2);

        if (!noConfirm)
        {
            string confirmation = TakeInput("Escribe \"si\" para confirmar: ");

            if (!string.Equals(confirmation, "si", StringComparison.OrdinalIgnoreCase))
            {
                Logs.Log(logTitle, $"Eliminación cancelada por el usuario (confirmación: '{confirmation}')", Logs.logType.Info, 2);
                DrawText("Cancelado");
                return;
            }
        }

        string userPath = Path.Combine(GPath, $"{userId}.json");

        User? user = LoadUserFromJson(userPath);

        if (user == null)
        {
            Logs.Log(logTitle, $"No se pudo cargar el usuario a eliminar en {userPath}", Logs.logType.Error, 3);
            DrawText($"Error cargando el archivo {userPath}", Color.Red);
            return;
        }

        if (File.Exists(userPath))
        {
            File.Delete(userPath);
            Logs.Log(logTitle, $"Archivo de usuario eliminado: {userPath}", Logs.logType.Info, 2);
        }

        RegenerateIndex.Run(GPath);
        Logs.Log(logTitle, $"Usuario {userId} ({user.name}) eliminado exitosamente", Logs.logType.Info, 2);
        DrawText("Usuario eliminado exitosamente!", Color.Green);
    }
}
