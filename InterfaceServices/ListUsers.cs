using System.Text.Json;
using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;
using static userdb.Services.UserService;

namespace userdb.InterfaceServices;

public static class List
{
    public static void InTable(List<string> userLines)
    {
        const string logTitle = "List.InTable";
        Logs.Log(logTitle, $"Renderizando tabla para {userLines.Count} registros de usuarios", Logs.logType.Info, 1);
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
        for (int i = 0; i < userLines.Count; i++)
        {
            List<string> userData = userLines[i].Split(',').ToList();

            if (userData.Count < 2)
            {
                Services.Logs.Log(logTitle, $"Línea invalida detectada en línea {i}, línea: {userLines[i]}", Services.Logs.logType.Error, 2);
                continue;
            }

            string jsonFile = userData[1];
            Services.Logs.Log(logTitle, $"Ruta de usuario leída: {jsonFile}", Services.Logs.logType.Info, 1);

            try
            {
                User? readedUser = LoadUserFromJson(jsonFile, true);
                if (readedUser == null) continue;

                List<string> roles = readedUser.wantedRoles ?? Array.Empty<string>().ToList();
                List<string> addRoles = readedUser.additionalRoles ?? Array.Empty<string>().ToList();
                List<string> pronounsList = readedUser.pronouns ?? Array.Empty<string>().ToList();

                if (roles.Count == 0) roles = new List<string> { "Ninguno" };
                if (addRoles.Count == 0) addRoles = new List<string> { "-" };
                if (pronounsList.Count == 0) pronounsList = new List<string> { "-" };

                int maxLines = Math.Max(roles.Count, Math.Max(addRoles.Count, pronounsList.Count));

                for (int line = 0; line < maxLines; line++)
                {
                    string idCol = (line == 0) ? Truncate(readedUser.userId, wId) : "";
                    string nameCol = (line == 0) ? Truncate(readedUser.name, wName) : "";
                    string fandomCol = (line == 0) ? Truncate(readedUser.fandom, wFandom) : "";
                    string ageCol = (line == 0) ? readedUser.age.ToString() : "";
                    string dateCol = (line == 0) ? readedUser.dateRegistered.ToString() : "";
                    string streakCol = (line == 0) ? readedUser.streak.ToString() : "";
                    string statusCol = (line == 0) ? Truncate(readedUser.status ?? "-", wStatus) : "";

                    string roleCol = (line < roles.Count) ? Truncate(roles[line], wRoles) : "";
                    string addRoleCol = (line < addRoles.Count) ? Truncate(addRoles[line], wAddRoles) : "";
                    string pronounCol = (line < pronounsList.Count) ? Truncate(pronounsList[line], wPronouns) : "";

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
                DrawText($"Archivo de usuario corrupto: {Path.GetFileName(jsonFile)}", Color.Red);
                Services.Logs.Log(logTitle, $"Archivo de usuario corrupto: {Path.GetFileName(jsonFile)}", Services.Logs.logType.Error, 3);
            }
        }
    }
}
