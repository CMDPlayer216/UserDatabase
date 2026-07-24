using System.Text.Json;
using NanoidDotNet;
using static userdb.ConsoleHelper;
using static userdb.UserService;

namespace userdb;

static class Commands
{
    public static void ListUsers(bool isTable, bool isRaw)
    {
        string[] userLines = GetUserIndexLines();

        if (userLines.Length == 0 && !isRaw)
        {
            DrawText("No hay usuarios registrados.", Color.Red);
            return;
        }

        if (isRaw)
        {
            string output = string.Join(Environment.NewLine, userLines);
            DrawText(output);
            return;
        }

        if (isTable)
        {
            // Definición de anchos fijos para columnas
            const int wId = 13;
            const int wName = 23;
            const int wFandom = 25;
            const int wAge = 5;
            const int wRoles = 20;
            const int wAddRoles = 20;
            const int wPronouns = 12;
            const int wDate = 12;
            const int wStreak = 6;
            const int wStatus = 15;

            // Renderizado del encabezado
            DrawText($"{Truncate("ID", wId),-wId}", Color.White, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{Truncate("USUARIO", wName),-wName}", Color.Green, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{Truncate("FANDOM", wFandom),-wFandom}", Color.White, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{Truncate("EDAD", wAge),-wAge}", Color.Green, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{Truncate("ROLES BUSCADOS", wRoles),-wRoles}", Color.White, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{Truncate("ROLES ADICIONALES", wAddRoles),-wAddRoles}", Color.Green, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{Truncate("PRONOMBRES", wPronouns),-wPronouns}", Color.White, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{Truncate("FECHA REG.", wDate),-wDate}", Color.Green, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{Truncate("RACHA", wStreak),-wStreak}", Color.White, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{Truncate("STATUS", wStatus),-wStatus}", Color.Green);

            int totalWidth = wId + wName + wFandom + wAge + wRoles + wAddRoles + wPronouns + wDate + wStreak + wStatus + 27;
            DrawText(new string('=', totalWidth), Color.Gray);

            // Iterar registros del índice
            for (int i = 0; i < userLines.Length; i++)
            {
                string[] userData = userLines[i].Split(',');
                if (userData.Length < 2) continue;

                string jsonFile = userData[1];

                try
                {
                    User? readedUser = LoadUserFromJson(jsonFile);
                    if (readedUser == null) continue;

                    string[] roles = readedUser.lookedCharacters ?? Array.Empty<string>();
                    string[] addRoles = readedUser.additionalRoles ?? Array.Empty<string>();
                    string[] pronounsList = readedUser.pronouns ?? Array.Empty<string>();

                    if (roles.Length == 0) roles = new[] { "Ninguno" };
                    if (addRoles.Length == 0) addRoles = new[] { "-" };
                    if (pronounsList.Length == 0) pronounsList = new[] { "-" };

                    int maxLines = Math.Max(roles.Length, Math.Max(addRoles.Length, pronounsList.Length));

                    for (int line = 0; line < maxLines; line++)
                    {
                        string idCol = (line == 0) ? Truncate(readedUser.userId, wId) : "";
                        string nameCol = (line == 0) ? Truncate(readedUser.name, wName) : "";
                        string fandomCol = (line == 0) ? Truncate(readedUser.fandom, wFandom) : "";
                        string ageCol = (line == 0) ? readedUser.age.ToString() : "";
                        string dateCol = (line == 0) ? readedUser.dateRegistered.ToString() : "";
                        string streakCol = (line == 0) ? readedUser.streak.ToString() : "";
                        string statusCol = (line == 0) ? Truncate(readedUser.status ?? "-", wStatus) : "";

                        string roleCol = (line < roles.Length) ? Truncate(roles[line], wRoles) : "";
                        string addRoleCol = (line < addRoles.Length) ? Truncate(addRoles[line], wAddRoles) : "";
                        string pronounCol = (line < pronounsList.Length) ? Truncate(pronounsList[line], wPronouns) : "";

                        DrawText($"{idCol,-wId}", Color.White, false);
                        DrawText(" | ", Color.Gray, false);
                        DrawText($"{nameCol,-wName}", Color.Green, false);
                        DrawText(" | ", Color.Gray, false);
                        DrawText($"{fandomCol,-wFandom}", Color.White, false);
                        DrawText(" | ", Color.Gray, false);
                        DrawText($"{ageCol,-wAge}", Color.Green, false);
                        DrawText(" | ", Color.Gray, false);
                        DrawText($"{roleCol,-wRoles}", Color.White, false);
                        DrawText(" | ", Color.Gray, false);
                        DrawText($"{addRoleCol,-wAddRoles}", Color.Green, false);
                        DrawText(" | ", Color.Gray, false);
                        DrawText($"{pronounCol,-wPronouns}", Color.White, false);
                        DrawText(" | ", Color.Gray, false);
                        DrawText($"{dateCol,-wDate}", Color.Green, false);
                        DrawText(" | ", Color.Gray, false);
                        DrawText($"{streakCol,-wStreak}", Color.White, false);
                        DrawText(" | ", Color.Gray, false);
                        DrawText($"{statusCol,-wStatus}", Color.Green);
                    }

                    DrawText(new string('-', totalWidth), Color.DarkGray);
                }
                catch (JsonException)
                {
                    DrawText($"[Archivo de usuario corrupto: {Path.GetFileName(jsonFile)}]", Color.Red);
                }
            }
        }
        else
        {
            foreach (string userline in userLines)
            {
                string[] user = userline.Split(',');
                if (user.Length > 1) DrawText(user[0]);
            }
        }
    }

    public static void AddUser(string name, string[] additionalRoles, string fandom, string[] lookedCharacters, int age, string[] pronouns, int streak, string id, string status)
    {
        User newUser = new User();
        newUser.name = name;
        newUser.additionalRoles = additionalRoles;
        newUser.fandom = fandom;
        newUser.lookedCharacters = lookedCharacters;
        newUser.age = age;
        newUser.pronouns = pronouns;
        newUser.streak = streak;
        newUser.status = status;
        if (id == "") id = Nanoid.Generate(size: 10);
        newUser.userId = id;
        newUser.dateRegistered = DateOnly.FromDateTime(DateTime.Now);
        newUser.status = status;
        SaveUser(newUser);
        DrawText("Usuario agregado exitosamente!", Color.Green);
    }

    public static void ModifyUser(string userId, string newName, string[] additionalRolesToAdd, string newFandom, string[] lookedCharactersToAdd, int newAge, string[] pronounsToAdd, int newStreak, string newId, string[] additionalRolesToRemove, string[] lookedCharactersToRemove, string[] pronounsToRemove, string newStatus)
    {
        User? user = LoadUserFromJson(Path.Combine(GPath, $"{userId}.json"));

        if (user == null)
        {
            DrawText($"No se pudo cargar la información del usuario en ({userId}.json).", Color.Red);
            return;
        }

        List<string> newAdditionalRoles = user.additionalRoles.ToList();
        List<string> newLookedCharacters = user.lookedCharacters.ToList();
        List<string> newPronouns = user.pronouns.ToList();

        // --- NOMBRE ---
        if (!string.IsNullOrWhiteSpace(newName)) user.name = newName;

        // --- ROLES ADICIONALES ---
        if (additionalRolesToAdd is { Length: > 0 }) newAdditionalRoles.AddRange(additionalRolesToAdd);

        if (additionalRolesToRemove is { Length: > 0 })
        {
            foreach (string rol in additionalRolesToRemove)
            {
                newAdditionalRoles.Remove(rol);
            }
        }
        user.additionalRoles = newAdditionalRoles.ToArray(); // Asegúrate de reasignar la lista convertida a array

        // --- FANDOM ---
        if (!string.IsNullOrWhiteSpace(newFandom))
            user.fandom = newFandom;

        // --- ROLES BUSCADOS (LOOKED CHARACTERS) ---
        if (lookedCharactersToAdd is { Length: > 0 })
            newLookedCharacters.AddRange(lookedCharactersToAdd);
        if (lookedCharactersToRemove is { Length: > 0 })
        {
            foreach (string character in lookedCharactersToRemove)
            {
                newLookedCharacters.Remove(character);
            }
        }
        user.lookedCharacters = newLookedCharacters.ToArray();

        // --- EDAD ---
        if (newAge > 0)
            user.age = newAge;

        // --- PRONOMBRES ---
        if (pronounsToAdd is { Length: > 0 })
            newPronouns.AddRange(pronounsToAdd);
        if (pronounsToRemove is { Length: > 0 })
        {
            foreach (string pronoun in pronounsToRemove)
            {

                newPronouns.Remove(pronoun);
            }
        }
        user.pronouns = newPronouns.ToArray();

        // --- RACHA ---
        if (newStreak >= 0) // Tip: 0 puede ser una racha válida si perdió la racha
            user.streak = newStreak;

        // --- USER ID Y MOVER ARCHIVO ---
        if (!string.IsNullOrEmpty(newId))
        {
            // Asegúrate de que las rutas apunten al directorio correcto usando Path.Combine si es necesario
            string oldPath = Path.Combine(GPath, $"{user.userId}.json");
            string newPath = Path.Combine(GPath, $"{newId}.json");

            if (File.Exists(oldPath))
            {
                File.Move(oldPath, newPath);
            }

            user.userId = newId;
        }

        if (!string.IsNullOrWhiteSpace(newStatus)) user.status = newStatus;

        UpdateUserJson(Path.Combine(GPath, $"{userId}.json"), user);
        DrawText("Usuario modificado con éxito!", Color.Green);
    }
    public static void DeleteUser(string userId, bool noConfirm = false)
    {
        if (!noConfirm)
        {
            string confirmation = TakeInput("Escribe \"si\" para confirmar: ");

            if (confirmation != "si")
            {
                DrawText("Cancelado");
                return;
            }
        }

        string userPath = Path.Combine(GPath, $"{userId}.json");

        User? user = LoadUserFromJson(userPath);

        if (user == null)
        {
            DrawText($"Error cargando el archivo {userPath}", Color.Red);
            return;
        }

        File.Delete(Path.Combine(GPath, $"{userId}.json"));
        List<string>? userIndex = File.ReadAllLines(Path.Combine(GPath, "users.dat")).ToList();

        userIndex.Remove($"{user.name},{userPath}");

        if (userIndex.ToArray().Length == 0)
        {
            File.Delete(Path.Combine(GPath, "users.dat"));
            return;
        }

        File.WriteAllLines(Path.Combine(GPath, "users.dat"), userIndex.ToArray());
    }
    public static void Show(string userId, bool isRaw = false)
    {
        string userPath = Path.Combine(GPath, $"{userId}.json");
        User? user = LoadUserFromJson(userPath);
        if (user == null)
        {
            DrawText($"Error al cargar el archivo {userPath}");
            return;
        }

        if (isRaw)
        {
            DrawText(File.ReadAllText(userPath));
            return;
        }

        DrawText($"Nombre: {user.name}");
        DrawText($"Roles adicionales: {string.Join(", ", user.additionalRoles)}");
        DrawText($"Fandom: {user.fandom}");
        DrawText($"Roles buscados: {string.Join(", ", user.lookedCharacters)}");
        DrawText($"Edad: {user.age}");
        DrawText($"Pronombres: {string.Join(", ", user.additionalRoles)}");
        DrawText($"Racha: {user.streak}");
        DrawText($"ID: {user.userId}");
        DrawText($"Status: {user.status}");
    }
}