using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;
using static userdb.Services.UserService;

namespace userdb.Commands;

public static class Show
{
    public static void Run(string userId, bool isRaw = false)
    {
        string logTitle = "Show";
        Logs.Log(logTitle, $"Consultando información de usuario '{userId}' (isRaw: {isRaw})", Logs.logType.Info, 2);

        string userPath = Path.Combine(GPath, $"{userId}.json");
        User? user = LoadUserFromJson(userPath);
        if (user == null)
        {
            Logs.Log(logTitle, $"No se pudo cargar el archivo de usuario: {userPath}", Logs.logType.Error, 3);
            DrawText($"Error al cargar el archivo {userPath}", Color.Red);
            return;
        }

        if (isRaw)
        {
            Logs.Log(logTitle, $"Mostrando JSON en crudo para {userId}", Logs.logType.Info, 1);
            DrawText(File.ReadAllText(userPath));
            return;
        }

        Logs.Log(logTitle, $"Mostrando datos formateados para usuario {userId} ({user.name})", Logs.logType.Info, 1);
        DrawText($"Nombre: {user.name}");
        DrawText($"Roles adicionales: {string.Join(", ", user.additionalRoles)}");
        DrawText($"Fandom: {user.fandom}");
        DrawText($"Roles buscados: {string.Join(", ", user.wantedRoles)}");
        DrawText($"Edad: {user.age}");
        DrawText($"Pronombres: {string.Join(", ", user.pronouns)}");
        DrawText($"Racha: {user.streak}");
        DrawText($"ID: {user.userId}");
        DrawText($"Status: {user.status}");
    }
}
