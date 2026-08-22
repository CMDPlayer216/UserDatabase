using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;
using static userdb.Services.UserService;

namespace userdb.Commands;

public static class ModifyUser
{
    public static void Run(ModifyingUser inputUser)
    {
        string logtitle = "ModifyUsers";
        inputUser.source = string.Concat(inputUser.source.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
        Logs.Log(logtitle, $"Leyendo usuario a modificar: {inputUser.source}", Logs.logType.Info, 3);
        User? user = LoadUserFromJson(Path.Combine(GPath, $"{inputUser.source}.json"));

        if (user == null)
        {
            Logs.Log(logtitle, $"No se pudo cargar el usuario ({inputUser.source}.json)", Logs.logType.Error, 3);
            DrawText($"No se pudo cargar la información del usuario en ({inputUser.source}.json).", Color.Red);
            return;
        }

        // --- NOMBRE ---
        if (!string.IsNullOrWhiteSpace(inputUser.name))
        {
            inputUser.name = string.Concat(inputUser.name.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
            Logs.Log(logtitle, $"Modificando nombre: '{user.name}' -> '{inputUser.name}'", Logs.logType.Info, 2);
            user.name = inputUser.name;
        }

        // --- ROLES ADICIONALES ---
        if (inputUser.additionalRolesToAdd is { Count: > 0 })
        {
            user.additionalRoles.AddRange(inputUser.additionalRolesToAdd);
            Logs.Log(logtitle, $"Roles adicionales añadidos: {string.Join(", ", inputUser.additionalRolesToAdd)}", Logs.logType.Info, 1);
        }

        if (inputUser.additionalRolesToRemove is { Count: > 0 })
        {
            foreach (string rol in inputUser.additionalRolesToRemove)
            {
                user.additionalRoles.Remove(rol);
            }
            Logs.Log(logtitle, $"Roles adicionales eliminados: {string.Join(", ", inputUser.additionalRolesToRemove)}", Logs.logType.Info, 1);
        }

        // --- FANDOM ---
        if (!string.IsNullOrWhiteSpace(inputUser.fandom))
        {
            Logs.Log(logtitle, $"Modificando fandom: '{user.fandom}' -> '{inputUser.fandom}'", Logs.logType.Info, 2);
            user.fandom = inputUser.fandom;
        }

        // --- ROLES BUSCADOS (LOOKED CHARACTERS) ---
        if (inputUser.wantedRolesToAdd is { Count: > 0 })
        {
            user.wantedRoles.AddRange(inputUser.wantedRolesToAdd);
            Logs.Log(logtitle, $"Roles buscados añadidos: {string.Join(", ", inputUser.wantedRolesToAdd)}", Logs.logType.Info, 1);
        }
        if (inputUser.wantedRolesToRemove is { Count: > 0 })
        {
            foreach (string character in inputUser.wantedRolesToRemove)
            {
                user.wantedRoles.Remove(character);
            }
            Logs.Log(logtitle, $"Roles buscados eliminados: {string.Join(", ", inputUser.wantedRolesToRemove)}", Logs.logType.Info, 1);
        }

        // --- EDAD ---
        if (inputUser.age > 0)
        {
            Logs.Log(logtitle, $"Modificando edad: {user.age} -> {inputUser.age}", Logs.logType.Info, 2);
            user.age = inputUser.age;
        }

        // --- PRONOMBRES ---
        if (inputUser.pronounsToAdd is { Count: > 0 })
        {
            user.pronouns.AddRange(inputUser.pronounsToAdd);
            Logs.Log(logtitle, $"Pronombres añadidos: {string.Join(", ", inputUser.pronounsToAdd)}", Logs.logType.Info, 1);
        }
        if (inputUser.pronounsToRemove is { Count: > 0 })
        {
            foreach (string pronoun in inputUser.pronounsToRemove)
            {
                user.pronouns.Remove(pronoun);
            }
            Logs.Log(logtitle, $"Pronombres eliminados: {string.Join(", ", inputUser.pronounsToRemove)}", Logs.logType.Info, 1);
        }

        // --- RACHA ---
        if (inputUser.streak >= 0)
        {
            Logs.Log(logtitle, $"Modificando racha: {user.streak} -> {inputUser.streak}", Logs.logType.Info, 2);
            user.streak = inputUser.streak;
        }

        // --- USER ID Y MOVER ARCHIVO ---
        if (!string.IsNullOrWhiteSpace(inputUser.userId) && inputUser.userId != user.userId)
        {
            inputUser.userId = string.Concat(inputUser.userId.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
            string oldPath = Path.Combine(GPath, $"{user.userId}.json");
            string newPath = Path.Combine(GPath, $"{inputUser.userId}.json");

            if (File.Exists(newPath) && newPath != oldPath)
            {
                int count = 1;
                while (File.Exists(Path.Combine(GPath, $"{inputUser.userId}{count}.json")))
                {
                    count++;
                }
                inputUser.userId = $"{inputUser.userId}{count}";
                newPath = Path.Combine(GPath, $"{inputUser.userId}.json");
            }

            if (File.Exists(oldPath) && oldPath != newPath)
            {
                File.Move(oldPath, newPath);
                Logs.Log(logtitle, $"Archivo movido de '{oldPath}' a '{newPath}'", Logs.logType.Info, 2);
            }

            user.userId = inputUser.userId;
        }

        if (!string.IsNullOrWhiteSpace(inputUser.status))
        {
            Logs.Log(logtitle, $"Modificando status: '{user.status}' -> '{inputUser.status}'", Logs.logType.Info, 2);
            user.status = inputUser.status;
        }

        UpdateUserJson(Path.Combine(GPath, $"{user.userId}.json"), user);
        RegenerateIndex.Run(GPath);
        Logs.Log(logtitle, $"Usuario {user.userId} ({user.name}) modificado exitosamente", Logs.logType.Info, 2);
        DrawText("Usuario modificado con éxito!", Color.Green);
    }
}
