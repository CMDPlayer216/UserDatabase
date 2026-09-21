using userdb.Models;
using userdb.Services;

namespace userdb.Commands;

public static class SearchUser
{
    public static void Run(SearchParameters searchOption)
    {
        const string logTitle = "SearchUser";
        UserService.CheckDatabaseLock(logTitle);
        bool fastSearch = searchOption.FastSearch
        && string.IsNullOrEmpty(searchOption.AdditionalRole)
        && string.IsNullOrEmpty(searchOption.Date)
        && string.IsNullOrEmpty(searchOption.Fandom)
        && string.IsNullOrEmpty(searchOption.MaxDate)
        && string.IsNullOrEmpty(searchOption.MinDate)
        && string.IsNullOrEmpty(searchOption.Pronoun)
        && string.IsNullOrEmpty(searchOption.Status)
        && string.IsNullOrEmpty(searchOption.WantedRole)
        && string.IsNullOrEmpty(searchOption.AdditionalRole)
        && searchOption.Age == -1
        && searchOption.MaxAge == -1
        && searchOption.MinAge == -1
        && searchOption.MaxStreak == -1
        && searchOption.MinStreak == -1
        && searchOption.Streak == -1;

        if (fastSearch)
        {
            List<string> results = [];
            List<string> index = UserService.GetUserIndexLines();
            if (index.Count == 0)
            {
                DrawText("Error: no hay usuarios registrados", Color.Red);
                Environment.Exit(1);
            }
            if (!searchOption.RawModifier)
            {
                foreach (string line in index)
                {
                    if (searchOption.Name != null && line.Split(',')[0] == searchOption.Name)
                    {
                        results.Add($"{Path.GetFileNameWithoutExtension(line.Split(',')[1])}   {line.Split(',')[0]}");
                        continue;
                    }
                    if (searchOption.Id != null && line.Split(',')[1] == searchOption.Id)
                    {
                        results.Add($"{Path.GetFileNameWithoutExtension(line.Split(',')[1])}   {line.Split(',')[0]}");
                        continue;
                    }
                    if (searchOption.Name != null && line.Contains(searchOption.Name))
                    {
                        results.Add($"{Path.GetFileNameWithoutExtension(line.Split(',')[1])}   {line.Split(',')[0]}");
                        continue;
                    }
                    if (searchOption.Id != null && line.Contains(searchOption.Id))
                    {
                        results.Add($"{Path.GetFileNameWithoutExtension(line.Split(',')[1])}   {line.Split(',')[0]}");
                        continue;
                    }
                }
            }
            else
            {
                foreach (string line in index)
                {
                    if (line.Contains(searchOption.Name ?? "") || line.Contains(searchOption.Id ?? "")) results.Add(line);
                }
            }

            results.Sort();
            if (searchOption.InverseOrder) results.Reverse();

            if (results.Count != 0)
            {
                foreach (string result in results)
                {
                    DrawText(result);
                }
            }
            if (results.Count == 0 && searchOption.RawModifier) return;
            if (results.Count == 0 && !searchOption.RawModifier)
            {
                DrawText("No hay nada que mostrar", Color.Red);
            }
        }
        else
        {
            List<string> results = [];
            List<string> index = UserService.GetUserIndexLines();
            RegenerateIndex.Run(UserService.GPath);

            foreach (string item in index)
            {
                User? user = UserService.LoadUserFromJson(item.Split(',')[1]);
                if (user == null) continue;

                string result = $"{Path.GetFileNameWithoutExtension(item.Split(',')[1])}   {item.Split(',')[0]}";
                if (searchOption.RawModifier) result = item;

                // Comprobaciones numéricas

                if (searchOption.Age != -1 && user.age == searchOption.Age)
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.MinAge != -1 && user.age >= searchOption.MinAge)
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.MaxAge != -1 && searchOption.MaxAge >= user.age)
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.Streak != -1 && searchOption.Streak == user.streak)
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.MinStreak != -1 && user.streak >= searchOption.MinStreak)
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.MaxStreak != -1 && searchOption.MaxStreak >= user.streak)
                {
                    results.Add(result);
                    continue;
                }

                // Comprobación exacta de cadenas

                if (searchOption.Name != null && user.name == searchOption.Name)
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.Id != null && user.userId == searchOption.Id)
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.Fandom != null && user.fandom == searchOption.Fandom)
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.Status != null && user.status == searchOption.Status)
                {
                    results.Add(result);
                    continue;
                }

                // Comprobación de fechas

                if (searchOption.Date != null && DateOnly.Parse(searchOption.Date) == user.dateRegistered)
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.MinDate != null && DateOnly.Parse(searchOption.MinDate) >= user.dateRegistered)
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.MaxDate != null && user.dateRegistered >= DateOnly.Parse(searchOption.MaxDate))
                {
                    results.Add(result);
                    continue;
                }

                // Comprobación de cadena en cadena

                if (searchOption.Name != null && user.name.Contains(searchOption.Name))
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.Id != null && user.userId.Contains(searchOption.Id))
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.Fandom != null && user.fandom.Contains(searchOption.Fandom))
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.Status != null && user.status.Contains(searchOption.Status))
                {
                    results.Add(result);
                    continue;
                }

                // Comprobación de cadena exacta dentro de lista

                if (searchOption.Pronoun != null && user.pronouns.Contains(searchOption.Pronoun))
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.WantedRole != null && user.wantedRoles.Contains(searchOption.WantedRole))
                {
                    results.Add(result);
                    continue;
                }
                if (searchOption.AdditionalRole != null && user.pronouns.Contains(searchOption.AdditionalRole))
                {
                    results.Add(result);
                    continue;
                }

                // Comprobación de cadena en cadena dentro de lista

                if (searchOption.Pronoun != null && user.pronouns != null && user.pronouns != Array.Empty<string>().ToList())
                {
                    bool match = false;
                    foreach (string role in user.pronouns)
                    {
                        if (role.Contains(searchOption.Pronoun))
                        {
                            results.Add(result);
                            match = true;
                            break;
                        }
                    }
                    if (match) continue;
                }
                if (searchOption.WantedRole != null && user.wantedRoles != null && user.wantedRoles != Array.Empty<string>().ToList())
                {
                    bool match = false;
                    foreach (string role in user.wantedRoles)
                    {
                        if (role.Contains(searchOption.WantedRole))
                        {
                            results.Add(result);
                            match = true;
                            break;
                        }
                    }
                    if (match) continue;
                }
                if (searchOption.AdditionalRole != null && user.additionalRoles != null && user.additionalRoles != Array.Empty<string>().ToList())
                {
                    bool match = false;
                    foreach (string role in user.additionalRoles)
                    {
                        if (role.Contains(searchOption.AdditionalRole))
                        {
                            results.Add(result);
                            match = true;
                            break;
                        }
                    }
                    if (match) continue;
                }
            }
            
            results.Sort();
            if (searchOption.InverseOrder) results.Reverse();

            if (results.Count != 0)
            {
                foreach (string result in results)
                {
                    DrawText(result);
                }
            }
            if (results.Count == 0 && searchOption.RawModifier) return;
            if (results.Count == 0 && !searchOption.RawModifier)
            {
                DrawText("No hay nada que mostrar", Color.Red);
            }
        }
    }
}
