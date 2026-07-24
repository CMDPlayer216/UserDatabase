using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace userdb;

public static class UserService
{
    public static string GPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".userdb");
    private static string UsersDatPath => Path.Combine(GPath, "users.dat");

    public static void EnsureDirectoryExists()
    {
        if (!Directory.Exists(GPath)) Directory.CreateDirectory(GPath);
    }

    public static string[] GetUserIndexLines()
    {
        if (!File.Exists(UsersDatPath)) return Array.Empty<string>();
        return File.ReadAllLines(UsersDatPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
    }

    public static void SaveUser(User newUser)
    {
        EnsureDirectoryExists();

        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        string userSerialized = JsonSerializer.Serialize(newUser, options);

        string path = Path.Combine(GPath, $"{newUser.userId}.json");

        if (File.Exists(path))
        {
            int count = 1;
            for (int i = 1; File.Exists(Path.Combine(GPath, $"{newUser.userId}{i}.json")); i++)
            {
                count = i + 1;
            }
            path = Path.Combine(GPath, $"{newUser.userId}{count}.json");
        }

        File.WriteAllText(path, userSerialized);
        File.AppendAllText(UsersDatPath, $"{newUser.name},{path}{Environment.NewLine}");
    }

    public static void UpdateUserJson(string jsonPath, User user)
    {
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        string updatedJson = JsonSerializer.Serialize(user, options);
        File.WriteAllText(jsonPath, updatedJson);
    }

    public static User? LoadUserFromJson(string jsonPath)
    {
        if (!File.Exists(jsonPath)) return null;
        string content = File.ReadAllText(jsonPath);
        return JsonSerializer.Deserialize<User>(content);
    }
}