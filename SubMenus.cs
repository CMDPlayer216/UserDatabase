using System.Text.Json;
using NanoidDotNet;
using static userdb.ConsoleHelper;

namespace userdb;

public static class Menus
{
    /// <summary>
    /// Renderiza la tabla de usuarios registrados en consola con formato multilínea.
    /// </summary>
    public static void ShowUsers(bool clearConsole = true)
    {
        if (clearConsole) Console.Clear();

        string[] userLines = UserService.GetUserIndexLines();

        if (userLines.Length == 0)
        {
            DrawText("No hay usuarios registrados.", Color.Red);
            return;
        }
        
        Commands.ListUsers(true, false);
    }

    /// <summary>
    /// Captura interactivamente datos del usuario, los valida y delega el guardado a UserService.
    /// </summary>
    public static void AddUser()
    {
        Console.Clear();

        string name;
        string[] additionalRoles;
        string fandom;
        string[] lookedCharacters;
        int age;
        string[] pronouns;
        int streak;
        string userId;

        string inp = "";
        string[] inpa = Array.Empty<string>();
        int inpi = 0;
        bool warning = false;

        // --- NOMBRE ---
        while (string.IsNullOrWhiteSpace(inp))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inp = TakeInput("Ingresa el nombre: ", Color.Yellow);
            inp = string.Concat(inp.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
            if (string.IsNullOrWhiteSpace(inp)) warning = true;
        }

        name = inp;
        warning = false;
        inp = "";

        // --- ROLES ADICIONALES ---
        inpa = TakeInput("Ingresa roles adicionales (separados por comas, puede estar vacío): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0])))
            inpa = Array.Empty<string>();

        additionalRoles = inpa;

        // --- FANDOM ---
        while (string.IsNullOrWhiteSpace(inp))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inp = TakeInput("Ingresa el fandom: ", Color.Yellow);
            if (string.IsNullOrWhiteSpace(inp)) warning = true;
        }

        fandom = inp;
        warning = false;
        inp = "";

        // --- ROLES BUSCADOS ---
        inpa = TakeInput("Ingresa los roles buscados (separados por comas, puede estar vacío): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0])))
            inpa = Array.Empty<string>();

        lookedCharacters = inpa;

        // --- EDAD ---
        while (string.IsNullOrWhiteSpace(inp) || inpi == 0)
        {
            if (warning) DrawText("Esa edad no es valida!", Color.Red);
            inp = TakeInput("Ingresa la edad: ", Color.Yellow);
            if (string.IsNullOrWhiteSpace(inp)) warning = true;
            if (!string.IsNullOrWhiteSpace(inp)) int.TryParse(inp, out inpi);
        }

        age = inpi;
        warning = false;
        inp = "";

        // --- PRONOMBRES ---
        inpa = Array.Empty<string>();
        while (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0])))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inpa = TakeInput("Ingresa los pronombres (separados por comas): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && string.IsNullOrWhiteSpace(inpa[0]))) warning = true;
        }

        pronouns = inpa;

        // --- RACHA ---
        string inputStreak = TakeInput("Ingresa la racha (de tenerla): ", Color.Yellow) ?? "0";
        if (!string.IsNullOrWhiteSpace(inputStreak))
            int.TryParse(inputStreak, out inpi);
        else
            inpi = 0;

        streak = inpi;

        // --- USER ID ---
        string inputId = TakeInput("Ingresa el ID (Deja vacío para autogenerar): ", Color.Yellow);
        inputId = string.Concat(inputId.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
        if (string.IsNullOrWhiteSpace(inputId))
            inputId = Nanoid.Generate(size: 12);

        userId = inputId;

        inp = TakeInput("Ingresa el status (Deja vacío para establecer en activo): ", Color.Yellow);
        inp = string.Concat(inp.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
        if (string.IsNullOrWhiteSpace(inp))
            inp = "Activo";
        string status = inp;
        
        // Guardar usuario vía Servicio
        Commands.AddUser(name, additionalRoles, fandom, lookedCharacters, age, pronouns, streak, userId, status);
    }

    /// <summary>
    /// Permite seleccionar un usuario y actualizar dinámicamente su racha diaria.
    /// </summary>
    public static void VerifyUsers()
    {
        Console.Clear();

        
        string[] lines = UserService.GetUserIndexLines();

        if (lines.Length == 0)
        {
            DrawText("No hay usuarios registrados para verificar.", Color.Red);
            return;
        }

        DrawText("SELECCIONA UN USUARIO:", Color.White);
        DrawText("");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] userData = lines[i].Split(',');
            DrawText($"{i + 1}. {userData[0]}", Color.Yellow);
        }

        DrawText("");
        string inputSelection = TakeInput("Ingresa el numero del usuario: ");

        if (!int.TryParse(inputSelection, out int selectedIndex) || selectedIndex < 1 || selectedIndex > lines.Length)
        {
            DrawText("Seleccion invalida.", Color.Red);
            return;
        }

        string[] selectedUserData = lines[selectedIndex - 1].Split(',');
        string jsonPath = selectedUserData[1];

        
        User? currentUser = UserService.LoadUserFromJson(jsonPath);

        if (currentUser == null)
        {
            DrawText($"No se pudo cargar la información del usuario en ({jsonPath}).", Color.Red);
            return;
        }

        DrawText($"Usuario seleccionado: {currentUser.name}", Color.White);
        DrawText("");
        DrawText($"Racha actual: {currentUser.streak}", Color.Gray);
        DrawText("");

        string respuesta = TakeInput("Realizo la actividad de hoy? (s/n): ", Color.Yellow).ToLower();

        if (respuesta == "s")
        {
            currentUser.streak += 1;
            DrawText($"Racha incrementada! Nueva racha: {currentUser.streak}", Color.Green);
        }
        else
        {
            currentUser.streak = 0;
            DrawText("Racha reiniciada a 0.", Color.Red);
        }

        
        UserService.UpdateUserJson(jsonPath, currentUser);
    }

    public static void ModifyUsers()
    {
        Console.Clear();

        
        string[] lines = UserService.GetUserIndexLines();

        if (lines.Length == 0)
        {
            DrawText("No hay usuarios registrados para modificar.", Color.Red);
            return;
        }

        DrawText("Selecciona un usuario: ", Color.White);
        DrawText("");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] userData = lines[i].Split(',');
            DrawText($"{i + 1}. {userData[0]}", Color.Yellow);
        }
        DrawText("");
        string inputSelection = TakeInput("Ingresa el numero del usuario: ");

        if (!int.TryParse(inputSelection, out int selectedIndex) || selectedIndex < 1 || selectedIndex > lines.Length)
        {
            DrawText("Seleccion invalida.", Color.Red);
            return;
        }

        int realindex = selectedIndex - 1;

        string[] selectedUserData = lines[realindex].Split(',');
        string jsonPath = selectedUserData[1];

        User? currentUser = UserService.LoadUserFromJson(jsonPath);

        if (currentUser == null)
        {
            DrawText($"No se pudo cargar la información del usuario en ({jsonPath}).", Color.Red);
            return;
        }

        string inp = currentUser.name;
        int inpi = 0;

        // --- NOMBRE DE USUARIO ---
        string newNameInput = TakeInput("Ingresa el nombre (deja vacío para conservar el original): ", Color.Yellow);

        if (!string.IsNullOrWhiteSpace(newNameInput))
        {
            string sanitizedName = string.Concat(newNameInput.Split(Path.GetInvalidFileNameChars())).Replace(",", "");

            currentUser.name = sanitizedName;

            // Actualizamos users.dat con el nuevo nombre manteniendo la misma ruta jsonPath
            lines[realindex] = $"{sanitizedName},{jsonPath}";
            File.WriteAllLines(Path.Combine(UserService.GPath, "users.dat"), lines);
        }

        // --- ROLES ADICIONALES ---
        string[] roles = currentUser.additionalRoles;
        bool erresable = true;
        bool loop = true;

        while (loop)
        {
            Console.Clear();

            DrawText("Roles actuales: ", Color.White);

            if (roles.Length != 0)
            {
                for (int i = 0; i < roles.Length; i++)
                {
                    DrawText($"{i + 1}. {roles[i]}");
                }
            }
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

            List<string> listRoles = roles.ToList();

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

                    while (inpi <= 0 || inpi > roles.Length)
                    {
                        if (warn) DrawText("Ese rol no existe!");
                        int.TryParse(TakeInput("Selecciona el índice del rol: "), out inpi);
                        if (inpi <= 0 || inpi > roles.Length) warn = true;
                    }
                    listRoles.RemoveAt(inpi - 1);
                    roles = listRoles.ToArray();
                    break;
                case "2":
                    listRoles.Add(TakeInput("Ingresa rol a añadir: "));
                    roles = listRoles.ToArray();
                    if (!erresable) erresable = true;
                    break;
                case "3":
                    currentUser.additionalRoles = roles;
                    loop = false;
                    break;
            }
        }

        // --- FANDOM ---
        Console.Clear();
        inp = TakeInput($"Ingresa el fandom (Actual: {currentUser.fandom}, deja vacío para conservar): ", Color.Yellow);
        if (!string.IsNullOrWhiteSpace(inp))
        {
            currentUser.fandom = inp;
        }

        // --- ROLES BUSCADOS ---
        roles = currentUser.lookedCharacters;
        erresable = true;
        loop = true;

        while (loop)
        {
            Console.Clear();

            DrawText("Roles buscados actuales: ", Color.White);

            if (roles.Length != 0)
            {
                for (int i = 0; i < roles.Length; i++)
                {
                    DrawText($"{i + 1}. {roles[i]}");
                }
            }
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

            List<string> listRoles = roles.ToList();

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

                    while (inpi <= 0 || inpi > roles.Length)
                    {
                        if (warn) DrawText("Ese rol no existe!");
                        int.TryParse(TakeInput("Selecciona el índice del rol: "), out inpi);
                        if (inpi <= 0 || inpi > roles.Length) warn = true;
                    }
                    listRoles.RemoveAt(inpi - 1);
                    roles = listRoles.ToArray();
                    break;
                case "2":
                    listRoles.Add(TakeInput("Ingresa rol a añadir: "));
                    roles = listRoles.ToArray();
                    if (!erresable) erresable = true;
                    break;
                case "3":
                    currentUser.lookedCharacters = roles;
                    loop = false;
                    break;
            }
        }

        // --- EDAD ---
        Console.Clear();
        inp = TakeInput($"Ingresa la edad (Actual: {currentUser.age}, deja vacío para conservar): ", Color.Yellow);
        if (int.TryParse(inp, out inpi) && inpi > 0)
        {
            currentUser.age = inpi;
        }

        // --- PRONOMBRES ---
        string[] pronouns = currentUser.pronouns;
        erresable = true;
        loop = true;

        while (loop)
        {
            Console.Clear();

            DrawText("Pronombres actuales: ", Color.White);

            for (int i = 0; i < pronouns.Length; i++)
            {
                DrawText($"{i + 1}. {pronouns[i]}");
            }

            DrawText("-------------------------------");
            DrawText("1. Eliminar pronombre");
            DrawText("2. Añadir pronombre");
            DrawText("3. Omitir");

            inp = TakeInput();

            List<string> listPronouns = pronouns.ToList();

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

                    while (inpi <= 0 || inpi > pronouns.Length)
                    {
                        if (warn) DrawText("Ese pronombre no existe!");
                        int.TryParse(TakeInput("Selecciona el índice del pronombre: "), out inpi);
                        if (inpi <= 0 || inpi > pronouns.Length) warn = true;
                    }
                    listPronouns.RemoveAt(inpi - 1);
                    pronouns = listPronouns.ToArray();
                    break;
                case "2":
                    listPronouns.Add(TakeInput("Ingresa pronombre a añadir: "));
                    pronouns = listPronouns.ToArray();
                    if (!erresable) erresable = true;
                    break;
                case "3":
                    currentUser.pronouns = pronouns;
                    loop = false;
                    break;
            }
        }

        // --- RACHA ---
        Console.Clear();
        inp = TakeInput($"Ingresa la racha (Actual: {currentUser.streak}, deja vacío para conservar): ", Color.Yellow);
        if (int.TryParse(inp, out inpi) && inpi >= 0)
        {
            currentUser.streak = inpi;
        }

        // --- STATUS ---
        Console.Clear();
        inp = TakeInput($"Ingresa el status (Actual: {currentUser.status}, deja vacío para conservar): ", Color.Yellow);
        if (!string.IsNullOrWhiteSpace(inp))
        {
            currentUser.status = inp;
        }

        // --- USER ID Y RENOMBRADO DE ARCHIVO .JSON ---
        Console.Clear();
        inp = TakeInput($"Ingresa la ID (Actual: {currentUser.userId}, deja vacío para conservar): ", Color.Yellow);
        if (!string.IsNullOrWhiteSpace(inp) && inp != currentUser.userId)
        {
            string newId = string.Concat(inp.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
            string oldPath = jsonPath;
            string newPath = Path.Combine(UserService.GPath, $"{newId}.json");

            if (File.Exists(newPath) && newPath != oldPath)
            {
                int count = 1;
                while (File.Exists(Path.Combine(UserService.GPath, $"{newId}{count}.json")))
                {
                    count++;
                }
                newId = $"{newId}{count}";
                newPath = Path.Combine(UserService.GPath, $"{newId}.json");
            }

            if (File.Exists(oldPath) && oldPath != newPath)
            {
                File.Move(oldPath, newPath);
            }

            currentUser.userId = newId;
            jsonPath = newPath;

            lines[realindex] = $"{currentUser.name},{jsonPath}";
            File.WriteAllLines(Path.Combine(UserService.GPath, "users.dat"), lines);
        }

        // Guardar usuario vía Servicio
        UserService.UpdateUserJson(jsonPath, currentUser);
        DrawText("¡Usuario modificado exitosamente!", Color.Green);
    }
    public static void RemoveUser()
    {
        Console.Clear();

        string[] lines = UserService.GetUserIndexLines();

        if (lines.Length == 0)
        {
            DrawText("No hay usuarios registrados para eliminar.", Color.Red);
            return;
        }

        DrawText("SELECCIONA UN USUARIO:", Color.White);
        DrawText("");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] userData = lines[i].Split(',');
            DrawText($"{i + 1}. {userData[0]}", Color.Yellow);
        }

        DrawText("");
        string inputSelection = TakeInput("Ingresa el numero del usuario: ");

        if (!int.TryParse(inputSelection, out int selectedIndex) || selectedIndex < 1 || selectedIndex > lines.Length)
        {
            DrawText("Seleccion invalida.", Color.Red);
            return;
        }

        string[] selectedUserData = lines[selectedIndex - 1].Split(',');

        User? user = UserService.LoadUserFromJson(selectedUserData[1]);

        if (user == null)
        {
            DrawText($"Error cargando el archivo {selectedUserData[1]}");
            return;
        }

        Commands.DeleteUser(user.userId);
    }

    private static void ImportUser()
    {
        Console.Clear();
        string source = TakeInput("Ruta del archivo JSON: ");
        DrawText("Seleccione un modo (en caso de duplicados), presione enter para dejar el por defecto: ");
        DrawText("1. Mantener los datos de cualquier usuario existente (por defecto)");
        DrawText("2. Sobreescribir TODOS los datos");
        DrawText("3. Combinar priorizando original");
        DrawText("4. Combinar priorizando nuevo");

        string input = TakeInput();
        int.TryParse(input, out int option);
        string mode = "keep";

        try
        {
            switch (option)
            {
                case 1:
                    mode = "keep";
                    break;
                case 2:
                    mode = "overwrite";
                    break;
                case 3:
                    mode = "combine-keeping-original";
                    break;
                case 4:
                    mode = "comine-keepeng-new";
                    break;
                default:
                    mode = "keep";
                    break;
            }
        }
        catch (Exception e)
        {
            DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
        }

        Commands.ImportSingleUser(source, mode);
    }
    private static void ExportUser()
    {
        Console.Clear();

        string[] lines = UserService.GetUserIndexLines();

        if (lines.Length == 0)
        {
            DrawText("No hay usuarios registrados para modificar.", Color.Red);
            return;
        }

        DrawText("Selecciona un usuario: ", Color.White);
        DrawText("");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] userData = lines[i].Split(',');
            DrawText($"{i + 1}. {userData[0]}", Color.Yellow);
        }
        DrawText("");
        string inputSelection = TakeInput("Ingresa el numero del usuario: ");

        if (!int.TryParse(inputSelection, out int selectedIndex) || selectedIndex < 1 || selectedIndex > lines.Length)
        {
            DrawText("Seleccion invalida.", Color.Red);
            return;
        }

        int realindex = selectedIndex - 1;
        string[] user = lines[realindex].Split(',');
        string dest = TakeInput("Ruta para exportar: ");

        User? userObj = UserService.LoadUserFromJson(user[1]);

        if (userObj == null)
        {
            DrawText("No se pudo cargar el usuario.", Color.Red);
            return;
        }

        Commands.ExportUser(userObj.userId, dest);
    }
    public static void UserImportOrExport()
    {
        Console.Clear();
        DrawText("Qué quieres hacer?");
        DrawText("1. Importar usuario");
        DrawText("2. Exportar usuario");
        DrawText("3. Volver");

        string input = TakeInput();
        int.TryParse(input, out int option);

        try
        {
            switch (option)
            {
                case 1:
                    ImportUser();
                    break;
                case 2:
                    ExportUser();
                    break;
                case 3:
                    return;
                default:
                    DrawText("Esa opcion no existe!", Color.Red);
                    break;
            }
        }
        catch (Exception e)
        {
            DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
        }
    }
    private static void ImportDataBase()
    {
        Console.Clear();
        string source = TakeInput("Ruta del archivo JSON: ");
        DrawText("Seleccione un modo (en caso de duplicados), presione enter para dejar el por defecto: ");
        DrawText("1. Mantener los datos de cualquier usuario existente (por defecto)");
        DrawText("2. Sobreescribir TODOS los datos");
        DrawText("3. Combinar priorizando original");
        DrawText("4. Combinar priorizando nuevo");

        string input = TakeInput();
        int.TryParse(input, out int option);
        string mode = "keep";

        try
        {
            switch (option)
            {
                case 1:
                    mode = "keep";
                    break;
                case 2:
                    mode = "overwrite";
                    break;
                case 3:
                    mode = "combine-keeping-original";
                    break;
                case 4:
                    mode = "comine-keepeng-new";
                    break;
                default:
                    mode = "keep";
                    break;
            }
        }
        catch (Exception e)
        {
            DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
        }

        if (!source.EndsWith(".userdb")) source += ".userdb";

        Commands.ImportDataBase(source, mode);
    }
    private static void ExportDataBase()
    {
        Console.Clear();
        string dest = TakeInput("Archivo de destino: ");
        Commands.ExportDataBase(dest);
    }
    public static void DataBaseImportOrExport()
    {
        Console.Clear();
        DrawText("Qué quieres hacer?");
        DrawText("1. Importar base de datos");
        DrawText("2. Exportar base de datos");
        DrawText("3. Volver");

        string input = TakeInput();
        int.TryParse(input, out int option);

        try
        {
            switch (option)
            {
                case 1:
                    ImportDataBase();
                    break;
                case 2:
                    ExportDataBase();
                    break;
                case 3:
                    return;
                default:
                    DrawText("Esa opcion no existe!", Color.Red);
                    break;
            }
        }
        catch (Exception e)
        {
            DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
        }
    }
}