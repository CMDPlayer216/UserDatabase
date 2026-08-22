using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;
using static userdb.Services.UserService;

namespace userdb.Commands;

public static class ExportUser
{
    public static void Run(string? userId, string? path)
    {
        string logTitle = "ExportUser";
        Logs.Log(logTitle, $"Iniciando exportación de usuario ID: '{userId}' a ruta: '{path}'", Logs.logType.Info, 2);

        string userPath = Path.Combine(UserService.GPath, $"{userId}.json");
        if (string.IsNullOrEmpty(path))
        {
            User? user = LoadUserFromJson(userPath);
            if (user == null)
            {
                Logs.Log(logTitle, $"Error al exportar: no se pudo cargar el usuario desde {userPath}", Logs.logType.Error, 3);
                DrawText("Error al crear el archivo: Archivo corrupto", Color.Red);
                return;
            }
            path = user.name + ".json";
        }
        if (path == null) path = $"User-Exported-{DateTime.Now.ToString()}.json";
        if (!path.EndsWith(".json")) path += ".json";
        string content = File.ReadAllText(userPath);
        File.WriteAllText(path, content);
        Logs.Log(logTitle, $"Usuario {userId} exportado con éxito a {path}", Logs.logType.Info, 2);
        DrawText($"Usuario exportado con éxito a: {path}", Color.Green);
    }
}
