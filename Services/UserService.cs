using System.Text.Json;
using userdb.Models;
using static userdb.ConsoleHelper;

namespace userdb.Services;

public static class UserService
{
    public static string GPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".userdb");
    public static string UsersDatPath => Path.Combine(GPath, "users.dat");

    public static void EnsureDirectoryExists()
    {
        if (!Directory.Exists(GPath))
        {
            Directory.CreateDirectory(GPath);
            Logs.Log("EnsureDirectoryExists", $"El directorio raíz {GPath} no existe, creando uno nuevo.", Logs.logType.Info, 2);
        }
    }

    public static List<string> GetUserIndexLines(bool regenerateIndex = true)
    {
        string logtitle = "GetUserIndexLines";
        if (regenerateIndex) RegenerateIndex.Run(UserService.GPath);
        if (!File.Exists(UsersDatPath))
        {
            Logs.Log(logtitle, "El archivo de índice no existe, a menos que no se hayan registrado usuarios, esto es un bug", Logs.logType.Warning, 2);
            return Array.Empty<string>().ToList();
        }
        return File.ReadAllLines(UsersDatPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
    }

    public static void SaveUser(User newUser)
    {
        EnsureDirectoryExists();

        string logTitle = "SaveUser";

        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };

        string path = Path.Combine(GPath, $"{newUser.userId}.json");
        Logs.Log(logTitle, $"Nueva ruta de usuario ({path}) será registrada", Logs.logType.Info, 2);

        if (File.Exists(path))
        {
            Logs.Log(logTitle, "La ruta existe, solucionando...", Logs.logType.Warning, 2);
            int count = 1;
            for (int i = 1; File.Exists(Path.Combine(GPath, $"{newUser.userId}{i}.json")); i++)
            {
                count = i + 1;
            }
            newUser.userId = $"{newUser.userId}{count}";
            path = Path.Combine(GPath, $"{newUser.userId}.json");
            Logs.Log(logTitle, $"Nueva ruta en: {path}", Logs.logType.Info, 2);
        }

        string userSerialized = JsonSerializer.Serialize(newUser, options);
        File.WriteAllText(path, userSerialized);
        Logs.Log(logTitle, "Actualizando índice...", Logs.logType.Info, 2);
        RegenerateIndex.Run(GPath);
        Logs.Log(logTitle, "Usuario agregado con éxito", Logs.logType.Info, 2);
    }

    public static void UpdateUserJson(string jsonPath, User user)
    {
        string logTitle = "UpdateUserJson";
        Logs.Log(logTitle, $"Actualizando archivo JSON de usuario en {jsonPath}", Logs.logType.Info, 2);
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        string updatedJson = JsonSerializer.Serialize(user, options);
        File.WriteAllText(jsonPath, updatedJson);
        Logs.Log(logTitle, $"Archivo {jsonPath} actualizado exitosamente", Logs.logType.Info, 2);
    }

    public static User? LoadUserFromJson(string jsonPath, bool skipIndex = false)
    {
        const string logTitle = "LoadUserFromJson";
        Logs.Log(logTitle, $"Cargando usuario desde {jsonPath}", Logs.logType.Info, 1);
        if (!skipIndex) RegenerateIndex.Run(UserService.GPath);

        if (!File.Exists(jsonPath))
        {
            Logs.Log(logTitle, "Usuario inválido detectado", Logs.logType.Error, 3);
            return null;
        }
        string content = File.ReadAllText(jsonPath);
        try
        {
            return JsonSerializer.Deserialize<User>(content);
        }
        catch (JsonException ex)
        {
            // El archivo no es un JSON o tiene mal formato
            DrawText($"Error de formato JSON: {ex.Message}", Color.Red);
            Logs.Log(logTitle, $"Error de formato JSON: {ex.Message}", Logs.logType.Error, 3);
            DrawText($"Línea: {ex.LineNumber}, Posición: {ex.BytePositionInLine}", Color.Red);
            Logs.Log(logTitle, $"Línea: {ex.LineNumber}, Posición: {ex.BytePositionInLine}", Logs.logType.Error, 3);
            return null;
        }
        catch (Exception ex)
        {
            DrawText($"Ocurrió un error inesperado: {ex.Message}", Color.Red);
            Logs.Log(logTitle, $"Ocurrió un error inesperado: {ex.Message}", Logs.logType.Error, 3);
            return null;
        }

    }
}