using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;
using static userdb.Services.UserService;

namespace userdb.Commands;

public static class ImportUser
{
    public static void Run(string source, string mode)
    {
        string logTitle = "ImportUser";
        Logs.Log(logTitle, $"Iniciando importación de usuario desde '{source}' con modo '{mode}'", Logs.logType.Info, 2);

        User? user = LoadUserFromJson(source);

        if (user == null)
        {
            Logs.Log(logTitle, $"Archivo de usuario no válido: {source}", Logs.logType.Error, 3);
            DrawText($"Omite importación: El archivo {Path.GetFileName(source)} no tiene formato válido.", Color.Red);
            return;
        }

        string userPath = Path.Combine(GPath, $"{user.userId}.json");
        string datPath = Path.Combine(GPath, "users.dat");

        switch (mode)
        {
            case "keep":
                if (File.Exists(Path.Combine(GPath, userPath)))
                {
                    Logs.Log(logTitle, $"Modo keep: El usuario {user.userId} ya existe, omitiendo", Logs.logType.Info, 2);
                    DrawText($"El usuario {user.userId} ya existe, omitiendo...", Color.Yellow);
                    return;
                }
                SaveUser(user);
                Logs.Log(logTitle, $"Modo keep: Usuario {user.userId} guardado", Logs.logType.Info, 2);
                break;

            case "overwrite":
                if (File.Exists(userPath))
                {
                    Logs.Log(logTitle, $"Modo overwrite: Sobrescribiendo usuario existente {user.userId}", Logs.logType.Info, 2);
                    File.Delete(userPath);

                    if (File.Exists(datPath))
                    {
                        List<string> userIndex = File.ReadAllLines(datPath).ToList();
                        userIndex.Remove($"{user.name},{userPath}");

                        if (userIndex.Count == 0) File.Delete(datPath);
                        else File.WriteAllLines(datPath, userIndex);
                    }
                }

                SaveUser(user);
                Logs.Log(logTitle, $"Modo overwrite: Usuario {user.userId} guardado", Logs.logType.Info, 2);
                break;

            case "combine-keeping-original":
                if (File.Exists(userPath))
                {
                    Logs.Log(logTitle, $"Modo combine-keeping-original: Combinando datos para usuario {user.userId}", Logs.logType.Info, 2);
                    ModifyingUser user2modify = new ModifyingUser();

                    user2modify.additionalRolesToAdd = user.additionalRoles;
                    user2modify.wantedRolesToAdd = user.wantedRoles;
                    user2modify.age = -1;
                    user2modify.pronounsToAdd = user.pronouns;
                    user2modify.source = user.userId;
                    user2modify.streak = -1;

                    ModifyUser.Run(user2modify);
                }
                else
                {
                    SaveUser(user);
                    Logs.Log(logTitle, $"Modo combine-keeping-original: Usuario nuevo {user.userId} guardado", Logs.logType.Info, 2);
                }
                break;

            case "combine-keeping-new":
                if (File.Exists(userPath))
                {
                    Logs.Log(logTitle, $"Modo combine-keeping-new: Combinando datos para usuario {user.userId}", Logs.logType.Info, 2);
                    ModifyingUser user2modify = new ModifyingUser();

                    user2modify.additionalRolesToAdd = user.additionalRoles;
                    user2modify.age = user.age;
                    user2modify.fandom = user.fandom;
                    user2modify.name = user.name;
                    user2modify.pronounsToAdd = user.pronouns;
                    user2modify.source = user.userId;
                    user2modify.status = user.status;
                    user2modify.streak = user.streak;
                    user2modify.userId = user.userId;
                    user2modify.wantedRolesToAdd = user.wantedRoles;

                    ModifyUser.Run(user2modify);
                }
                else
                {
                    SaveUser(user);
                    Logs.Log(logTitle, $"Modo combine-keeping-new: Usuario nuevo {user.userId} guardado", Logs.logType.Info, 2);
                }
                break;
        }
    }
}
