using userdb.Commands;
using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus;

public static class ModifyUserMenu
{
    public static void Show()
    {
        string logTitle = "ModifyUserMenu";
        Logs.Log(logTitle, "Mostrando menú interactivo para modificar usuario", Logs.logType.Info, 2);
        Console.Clear();
        List<string> lines = UserService.GetUserIndexLines();

        if (lines.Count == 0)
        {
            Logs.Log(logTitle, "No hay usuarios registrados para modificar", Logs.logType.Info, 1);
            DrawText("No hay usuarios registrados para modificar.", Color.Red);
            return;
        }

        DrawText("Selecciona un usuario: ", Color.White);
        DrawText("");

        for (int i = 0; i < lines.Count; i++)
        {
            List<string> userData = lines[i].Split(',').ToList();
            DrawText($"{i + 1}. {userData[0]}", Color.Yellow);
        }
        DrawText("");
        string inputSelection = TakeInput("Ingresa el numero del usuario: ");

        if (!int.TryParse(inputSelection, out int selectedIndex) || selectedIndex < 1 || selectedIndex > lines.Count)
        {
            DrawText("Seleccion invalida.", Color.Red);
            return;
        }

        int realindex = selectedIndex - 1;

        List<string> selectedUserData = lines[realindex].Split(',').ToList();
        string jsonPath = selectedUserData[1];
        ModifyingUser currentUser = new();
        User? user = UserService.LoadUserFromJson(jsonPath);

        if (user == null)
        {
            DrawText("Error leyendo usuario", Color.Red);
            return;
        }

        currentUser.source = Path.GetFileNameWithoutExtension(jsonPath);
        Logs.Log(logTitle, $"ID de usuario seleccionado: {currentUser.source}", Logs.logType.Info, 2);

        string inp = currentUser.name;
        int inpi = 0;

        currentUser.name = TakeInput("Ingresa el nombre (deja vacío para conservar el original): ", Color.Yellow);

        bool erresable = true;
        bool loop = true;

        while (loop)
        {
            Console.Clear();

            DrawText("Roles actuales: ", Color.White);

            if (user.additionalRoles.Count != 0) for (int i = 0; i < user.additionalRoles.Count; i++) DrawText($"{i + 1}. {user.additionalRoles[i]}");
            else
            {
                DrawText("No hay roles registrados");
                erresable = false;
            }

            DrawText("-------------------------------");
            DrawText("1. Eliminar rol");
            DrawText("2. Añadir rol");
            DrawText("3. Omitir");

            inp = TakeInput();

            switch (inp)
            {
                case "1":
                    if (!erresable)
                    {
                        DrawText("No hay roles por eliminar", Color.Red);
                        break;
                    }
                    bool warn = false;
                    inpi = 0;

                    while (inpi <= 0 || inpi > user.additionalRoles.Count)
                    {
                        if (warn) DrawText("Ese rol no existe!");
                        int.TryParse(TakeInput("Selecciona el índice del rol: "), out inpi);
                        if (inpi <= 0 || inpi > user.additionalRoles.Count) warn = true;
                    }
                    currentUser.additionalRolesToRemove.Add(user.additionalRoles[inpi - 1]);
                    user.additionalRoles.RemoveAt(inpi - 1);
                    break;
                case "2":
                    currentUser.additionalRolesToAdd.Add(TakeInput("Ingresa rol a añadir: "));
                    if (!erresable) erresable = true;
                    break;
                case "3":
                    loop = false;
                    break;
            }
        }

        Console.Clear();
        currentUser.fandom = TakeInput($"Ingresa el fandom (Actual: {user.fandom}, deja vacío para conservar): ", Color.Yellow);

        erresable = true;
        loop = true;

        while (loop)
        {
            Console.Clear();

            DrawText("Roles buscados actuales: ", Color.White);

            if (user.wantedRoles.Count != 0) for (int i = 0; i < user.wantedRoles.Count; i++) DrawText($"{i + 1}. {user.wantedRoles[i]}");

            else
            {
                DrawText("No hay roles registrados");
                erresable = false;
            }

            DrawText("-------------------------------");
            DrawText("1. Eliminar rol");
            DrawText("2. Añadir rol");
            DrawText("3. Omitir");

            inp = TakeInput();

            switch (inp)
            {
                case "1":
                    if (!erresable)
                    {
                        DrawText("No hay roles por eliminar", Color.Red);
                        break;
                    }
                    bool warn = false;
                    inpi = 0;

                    while (inpi <= 0 || inpi > user.wantedRoles.Count)
                    {
                        if (warn) DrawText("Ese rol no existe!");
                        int.TryParse(TakeInput("Selecciona el índice del rol: "), out inpi);
                        if (inpi <= 0 || inpi > user.wantedRoles.Count) warn = true;
                    }
                    currentUser.wantedRolesToRemove.Add(user.wantedRoles[inpi - 1]);
                    user.wantedRoles.RemoveAt(inpi - 1);
                    break;
                case "2":
                    currentUser.wantedRolesToAdd.Add(TakeInput("Ingresa rol a añadir: "));
                    if (!erresable) erresable = true;
                    break;
                case "3":
                    loop = false;
                    break;
            }
        }

        // --- EDAD ---
        Console.Clear();
        string intToConvert = TakeInput($"Ingresa la edad (Actual: {user.age}, deja vacío para conservar): ", Color.Yellow);
        if (!string.IsNullOrWhiteSpace(intToConvert) && int.TryParse(intToConvert, out int ageVal) && ageVal > 0)
        {
            currentUser.age = ageVal;
        }
        else
        {
            currentUser.age = 0;
        }

        // --- PRONOMBRES ---
        erresable = true;
        loop = true;

        while (loop)
        {
            Console.Clear();

            DrawText("Pronombres actuales: ", Color.White);

            for (int i = 0; i < user.pronouns.Count; i++)
            {
                DrawText($"{i + 1}. {user.pronouns[i]}");
            }

            DrawText("-------------------------------");
            DrawText("1. Eliminar pronombre");
            DrawText("2. Añadir pronombre");
            DrawText("3. Omitir");

            inp = TakeInput();

            switch (inp)
            {
                case "1":
                    if (!erresable)
                    {
                        DrawText("No hay pronombre por eliminar", Color.Red);
                        break;
                    }
                    bool warn = false;
                    inpi = 0;

                    while (inpi <= 0 || inpi > user.pronouns.Count)
                    {
                        if (warn) DrawText("Ese pronombre no existe!");
                        int.TryParse(TakeInput("Selecciona el índice del pronombre: "), out inpi);
                        if (inpi <= 0 || inpi > user.pronouns.Count) warn = true;
                    }
                    currentUser.pronounsToRemove.Add(user.pronouns[inpi - 1]);
                    user.pronouns.RemoveAt(inpi - 1);
                    break;
                case "2":
                    currentUser.pronounsToAdd.Add(TakeInput("Ingresa pronombre a añadir: "));
                    if (!erresable) erresable = true;
                    break;
                case "3":
                    loop = false;
                    break;
            }
        }

        // --- RACHA ---
        Console.Clear();
        intToConvert = TakeInput($"Ingresa la racha (Actual: {user.streak}, deja vacío para conservar): ", Color.Yellow);
        if (!string.IsNullOrWhiteSpace(intToConvert) && int.TryParse(intToConvert, out int streakVal) && streakVal >= 0)
        {
            currentUser.streak = streakVal;
        }
        else
        {
            currentUser.streak = -1;
        }

        // --- STATUS ---
        Console.Clear();
        currentUser.status = TakeInput($"Ingresa el status (Actual: {user.status}, deja vacío para conservar): ", Color.Yellow);

        // --- USER ID Y RENOMBRADO DE ARCHIVO .JSON ---
        Console.Clear();
        currentUser.userId = TakeInput($"Ingresa la ID (Actual: {user.userId}, deja vacío para conservar): ", Color.Yellow);
        
        // Guardar usuario vía Servicio
        ModifyUser.Run(currentUser);
    }
}
