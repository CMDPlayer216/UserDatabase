using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace _userdatabase.UserDatabase;

public static class Program
{
    // Se crea el directorio raíz de la aplicación según el SO lo mande
    public static string gpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".userdatabase_data");

    // Objeto principal donde se guardarán los datos de los usuarios
    private class User
    {
        /* Nombre de usuario, se usa para guardar el rol pricipal, apodo o nombre de
           usuario con el que se mostrará en la lista, además de que definirá el
           nombre del archivo más adelante */
        public string name { get; set; } = "";

        // ID de usuario, sirve para diferenciar usuarios en la lista
        public string userId { get; set; } = "";

        // Lista de roles adicionales que se le atribuyan a este usuario
        [JsonPropertyName("aditionalRoles")]
        public string[] additionalRoles { get; set; } = Array.Empty<string>();

        // Edad del usuario
        public int age { get; set; }

        // Fandom al que el usuario pertenece
        public string fandom { get; set; } = "";

        // Personajes que busca el usuario para rolear
        public string[] lookedCharacters { get; set; } = Array.Empty<string>();

        // Pronombres que usa el usuario
        [JsonPropertyName("pronauns")]
        public string[] pronouns { get; set; } = Array.Empty<string>();

        // Fecha en que se registró el usuario
        public DateOnly dateRegistered { get; set; }

        // Racha de actividades del usuario
        public int streak { get; set; }
    }

    // Método para cambiar facilmente el color de la consola
    private static void ColorSwitch(Color color)
    {
        Console.ForegroundColor = color switch
        {
            Color.White => ConsoleColor.White,
            Color.Black => ConsoleColor.Black,
            Color.Yellow => ConsoleColor.Yellow,
            Color.Green => ConsoleColor.Green,
            Color.Cyan => ConsoleColor.Cyan,
            Color.Blue => ConsoleColor.Blue,
            Color.DarkYellow => ConsoleColor.DarkYellow,
            Color.Red => ConsoleColor.Red,
            Color.Gray => ConsoleColor.Gray,
            Color.DarkGreen => ConsoleColor.DarkGreen,
            Color.DarkGray => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };

        if (color == Color.Default)
        {
            Console.ResetColor();
        }
    }

    private enum Color
    {
        Default,
        White,
        Black,
        Yellow,
        Green,
        Cyan,
        Blue,
        DarkYellow,
        Red,
        Gray,
        DarkGreen,
        DarkGray
    }

    // Método principal para dibujar texto en consola
    private static void DrawText(string text, Color color = Color.Default, bool InsertNewLine = true)
    {
        ColorSwitch(color);

        Console.Write(text);

        if (InsertNewLine) Console.Write(Environment.NewLine);

        Console.ResetColor();
    }

    // Método usado para limpiar texto donde sea necesario
    private static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text.Length <= maxLength ? text : text.Substring(0, maxLength - 3) + "...";
    }

    /// <summary>
    /// Método para mostrar tabla de usuarios en consola (Posiblemente el método más complejo de este programa)
    /// </summary>
    private static void ShowUsers()
    {
        Console.Clear();

        // Obtener la ruta del archivo índice de usuarios
        string usersDatPath = Path.Combine(gpath, "users.dat");

        // Validar si el archivo índice existe y contiene registros válidos
        if (!File.Exists(usersDatPath) || string.IsNullOrWhiteSpace(File.ReadAllText(usersDatPath)))
        {
            DrawText("No hay usuarios registrados.", Color.Red);
            return;
        }

        // Definición de anchos fijos (en número de caracteres) para cada columna de la tabla
        const int wId = 24;
        const int wName = 25;
        const int wFandom = 25;
        const int wAge = 5;
        const int wRoles = 20;
        const int wAddRoles = 20;
        const int wPronouns = 12;
        const int wDate = 12;
        const int wStreak = 6;

        // Renderizado de los títulos del encabezado de la tabla
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
        DrawText($"{Truncate("RACHA", wStreak),-wStreak}", Color.White);

        // Calcular el ancho total de la tabla (columnas + separadores ' | ') y dibujar la línea principal
        int totalWidth = wId + wName + wFandom + wAge + wRoles + wAddRoles + wPronouns + wDate + wStreak + 24;
        DrawText(new string('=', totalWidth), Color.Gray);

        // Cargar y filtrar las líneas no vacías del archivo índice
        string[] Users = File.ReadAllLines(usersDatPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

        // Iterar cada usuario registrado en el índice
        for (int i = 0; i < Users.Length; i++)
        {
            string[] user = Users[i].Split(',');
            if (user.Length < 2) continue; // Omitir registros mal formados en users.dat

            string jsonFile = user[1];

            if (!File.Exists(jsonFile)) continue; // Omitir si la ficha JSON del usuario fue eliminada

            try
            {
                // Leer y deserializar los datos del usuario desde su archivo JSON
                string deserializedUser = File.ReadAllText(jsonFile);
                User? readedUser = JsonSerializer.Deserialize<User>(deserializedUser);

                if (readedUser == null) continue;

                // Extraer las colecciones dinámicas (listas) del usuario
                string[] roles = readedUser.lookedCharacters ?? Array.Empty<string>();
                string[] addRoles = readedUser.additionalRoles ?? Array.Empty<string>();
                string[] pronounsList = readedUser.pronouns ?? Array.Empty<string>();

                // Asignar placeholders por defecto si alguna lista está vacía
                if (roles.Length == 0) roles = new[] { "Ninguno" };
                if (addRoles.Length == 0) addRoles = new[] { "-" };
                if (pronounsList.Length == 0) pronounsList = new[] { "-" };

                // Determinar la altura (en líneas) que requerirá la fila según la lista más larga
                int maxLines = Math.Max(roles.Length, Math.Max(addRoles.Length, pronounsList.Length));

                // Bucle interno para construir y renderizar el formato multilínea del usuario
                for (int line = 0; line < maxLines; line++)
                {
                    // Los datos fijos solo se imprimen en la primera sub-línea (line 0); en las demás se deja espacio en blanco
                    string idCol = (line == 0) ? Truncate(readedUser.userId, wId) : "";
                    string nameCol = (line == 0) ? Truncate(readedUser.name, wName) : "";
                    string fandomCol = (line == 0) ? Truncate(readedUser.fandom, wFandom) : "";
                    string ageCol = (line == 0) ? readedUser.age.ToString() : "";
                    string dateCol = (line == 0) ? readedUser.dateRegistered.ToString() : "";
                    string streakCol = (line == 0) ? readedUser.streak.ToString() : "";

                    // Imprimir el elemento correspondiente de las listas desplegables (o vacío si la lista ya terminó)
                    string roleCol = (line < roles.Length) ? Truncate(roles[line], wRoles) : "";
                    string addRoleCol = (line < addRoles.Length) ? Truncate(addRoles[line], wAddRoles) : "";
                    string pronounCol = (line < pronounsList.Length) ? Truncate(pronounsList[line], wPronouns) : "";

                    // Dibujar la sub-fila formateada en consola con sus respectivos colores
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
                    DrawText($"{streakCol,-wStreak}", Color.White);
                }

                // Dibujar una línea divisora tenue entre cada usuario
                DrawText(new string('-', totalWidth), Color.DarkGray);
            }
            catch (JsonException)
            {
                // Manejo de excepción si la ficha JSON individual tiene sintaxis corrupta
                DrawText($"[Archivo de usuario corrupto: {Path.GetFileName(jsonFile)}]", Color.Red);
            }
        }
    }
    /// <summary>
    /// Solicita los datos interactivos por consola, valida la entrada, instancia un objeto User
    /// y lo guarda en formato JSON junto a su registro en el archivo índice.
    /// </summary>
    private static void AddUser()
    {
        User newUser = new User();

        Console.Clear();

        // Variables de trabajo auxiliares para la lectura y validación de datos
        string inp = "";
        string[] inpa = [""];
        int inpi = 0;
        bool warning = false;

        // --- CAPTURA: NOMBRE DE USUARIO ---
        // Solicita un nombre válido eliminando caracteres no permitidos para rutas de archivos o CSV
        while (string.IsNullOrEmpty(inp) || string.IsNullOrWhiteSpace(inp))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inp = TakeInput("Ingresa el nombre: ", Color.Yellow);

            // Limpia caracteres inválidos para nombres de archivo y comas para evitar conflictos con .dat
            inp = string.Concat(inp.Split(Path.GetInvalidFileNameChars())).Replace(",", "");
            if (string.IsNullOrEmpty(inp) || string.IsNullOrWhiteSpace(inp)) warning = true;
        }

        newUser.name = inp;

        warning = false;
        inp = "";

        // --- CAPTURA: ROLES ADICIONALES ---
        // Convierte la cadena separada por comas en un arreglo recortando espacios y omitiendo entradas vacías
        inpa = TakeInput("Ingresa roles adicionales (separados por comas, puede estar vacío): ", Color.Yellow)
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && (string.IsNullOrEmpty(inpa[0]) || string.IsNullOrWhiteSpace(inpa[0]))))
            inpa = ["Ninguno"];

        newUser.additionalRoles = inpa;

        inpa = [""];

        // --- CAPTURA: FANDOM ---
        while (string.IsNullOrEmpty(inp) || string.IsNullOrWhiteSpace(inp))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inp = TakeInput("Ingresa el fandom: ", Color.Yellow);
            if (string.IsNullOrEmpty(inp) || string.IsNullOrWhiteSpace(inp)) warning = true;
        }

        newUser.fandom = inp;

        warning = false;
        inp = "";

        // --- CAPTURA: ROLES BUSCADOS ---
        inpa = TakeInput("Ingresa los roles buscados (separados por comas, puede estar vacío): ", Color.Yellow)
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && (string.IsNullOrEmpty(inpa[0]) || string.IsNullOrWhiteSpace(inpa[0]))))
            inpa = ["Ninguno"];

        newUser.lookedCharacters = inpa;

        inpa = [""];

        // --- CAPTURA: EDAD ---
        // Valida que el valor ingresado no sea vacío y sea parseable a un número mayor a cero
        while (string.IsNullOrEmpty(inp) || string.IsNullOrWhiteSpace(inp) || inpi == 0)
        {
            if (warning) DrawText("Esa edad no es valida!", Color.Red);
            inp = TakeInput("Ingresa la edad: ", Color.Yellow);
            if (string.IsNullOrEmpty(inp) || string.IsNullOrWhiteSpace(inp)) warning = true;
            if (!string.IsNullOrEmpty(inp) || !string.IsNullOrWhiteSpace(inp)) int.TryParse(inp, out inpi);
        }

        warning = false;
        inp = "";

        newUser.age = inpi;

        // --- CAPTURA: PRONOMBRES ---
        // Exige al menos un pronombre válido
        while (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && (string.IsNullOrEmpty(inpa[0]) || string.IsNullOrWhiteSpace(inpa[0]))))
        {
            if (warning) DrawText("No puedes dejar el campo vacío!", Color.Red);
            inpa = TakeInput("Ingresa los pronombres (separados por comas): ", Color.Yellow)
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (inpa == null || inpa.Length == 0 || (inpa.Length == 1 && (string.IsNullOrEmpty(inpa[0]) || string.IsNullOrWhiteSpace(inpa[0])))) warning = true;
        }

        newUser.pronouns = inpa;

        // --- CAPTURA: RACHA ---
        // Asigna 0 por defecto si no se ingresa un valor entero válido
        string inputStreak = TakeInput("Ingresa la racha (de tenerla): ", Color.Yellow) ?? "0";

        if (!string.IsNullOrEmpty(inputStreak) || !string.IsNullOrWhiteSpace(inputStreak))
            int.TryParse(inputStreak, out inpi);
        else
            inpi = 0;

        newUser.streak = inpi;

        // --- CAPTURA / GENERACIÓN: USER ID ---
        // Utiliza el ID ingresado o autogenera un UUID v4 si se deja en blanco
        string inputId = TakeInput("Ingresa el ID (Deja vacío para autogenerar): ", Color.Yellow);

        Guid uuid = Guid.NewGuid();
        string textuuid = uuid.ToString();

        if (string.IsNullOrEmpty(inputId) || string.IsNullOrWhiteSpace(inputId))
            inputId = textuuid;

        newUser.userId = inputId;

        // Asignar la fecha actual del sistema
        newUser.dateRegistered = DateOnly.FromDateTime(DateTime.Now);

        // --- SERIALIZACIÓN Y GUARDADO ---
        // Formatea el JSON con sangría para facilitar lectura/edición manual
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        string UserSerialized = JsonSerializer.Serialize(newUser, options);

        string path = Path.Combine(gpath, $"{newUser.name}.json");

        // Resolver colisión de nombres de archivos si el usuario ya existe (ej: Juan.json -> Juan1.json)
        if (File.Exists(path))
        {
            int count = 1;
            for (int i = 1; File.Exists(Path.Combine(gpath, $"{newUser.name}{i}.json")); i++)
            {
                count = i + 1;
            }
            path = Path.Combine(gpath, $"{newUser.name}{count}.json");
        }

        // Escribir el archivo JSON individual y actualizar el índice general users.dat
        File.WriteAllText(path, UserSerialized);
        File.AppendAllText(Path.Combine(gpath, "users.dat"), $"{newUser.name},{path}{Environment.NewLine}");
    }
    private static string TakeInput(string prefix = " > ", Color prefixColor = Color.Green, Color textColor = Color.Default)
    {
        ColorSwitch(prefixColor);

        Console.Write(prefix);

        ColorSwitch(textColor);

        string s = Console.ReadLine() ?? "";

        Console.ResetColor();

        DrawText("");

        return s;
    }

    private static void VerifyUsers()
    {
        Console.Clear();

        string usersDatPath = Path.Combine(gpath, "users.dat");

        if (!File.Exists(usersDatPath) || string.IsNullOrWhiteSpace(File.ReadAllText(usersDatPath)))
        {
            DrawText("No hay usuarios registrados para verificar.", Color.Red);
            return;
        }

        string[] lines = File.ReadAllLines(usersDatPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

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
            DrawText("Seleccion invalida.");
            return;
        }

        string[] selectedUserData = lines[selectedIndex - 1].Split(',');
        string jsonPath = selectedUserData[1];

        if (!File.Exists(jsonPath))
        {
            DrawText($"No se encontro el archivo de datos ({jsonPath}).");
            return;
        }

        string deserializedUser = File.ReadAllText(jsonPath);
        User? currentUser = JsonSerializer.Deserialize<User>(deserializedUser);

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

        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        string updatedJson = JsonSerializer.Serialize(currentUser, options);
        File.WriteAllText(jsonPath, updatedJson);
    }
    public static void Main()
    {
        if (!Directory.Exists(gpath))
        {
            Directory.CreateDirectory(gpath);
        }
        string input = "";
        Console.Clear();

        DrawText(" _   _ ____  _____ ____    ____    _  _____  _    ____    _    ____  _____ ");
        DrawText("| | | / ___|| ____|  _ \\  |  _ \\  / \\|_   _|/ \\  | __ )  / \\  / ___|| ____|");
        DrawText("| | | \\___ \\|  _| | |_) | | | | |/ _ \\ | | / _ \\ |  _ \\ / _ \\ \\___ \\|  _|  ");
        DrawText("| |_| |___) | |___|  _ <  | |_| / ___ \\| |/ ___ \\| |_) / ___ \\ ___) | |___");
        DrawText(" \\___/|____/|_____|_| \\_\\ |____/_/   \\_\\_/_/   \\_\\____/_/   \\_\\____/|_____|");
        DrawText("");
        DrawText("UserDB v1.3 - Copyright (c) 2026 CMDPlayer216", Color.Gray);

        while (true)
        {
            DrawText("");
            DrawText("1. Mostrar usuarios");
            DrawText("2. Agregar un usuario");
            DrawText("3. Verificar un usuario");
            DrawText("4. Salir");
            DrawText("");
            input = TakeInput();

            int.TryParse(input, out int intinput);

            try
            {
                switch (intinput)
                {
                    case 1:
                        ShowUsers();
                        break;
                    case 2:
                        AddUser();
                        break;
                    case 3:
                        VerifyUsers();
                        break;
                    case 4:
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
}