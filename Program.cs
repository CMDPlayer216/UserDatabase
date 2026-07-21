using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace _userdatabase.UserDatabase;

public static class Program
{
    public static string gpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".userdatabase_data");
    private class User
    {
        public string name { get; set; } = "";
        public int age { get; set; }
        public string fandom { get; set; } = "";
        public string[] lookedCharacters { get; set; } = [""];
        public string[] pronauns { get; set; } = [""];
        public DateOnly dateRegistered { get; set; }
        public int streak { get; set; }
    }
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
            _ => ConsoleColor.White // default fallback
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
        DarkGreen
    }
    private static void DrawText(string text, Color color = Color.Default, bool InsertNewLine = true)
    {
        ColorSwitch(color);

        Console.Write(text);

        if (InsertNewLine) Console.Write("\n");

        Console.ResetColor();
    }

    private static void ShowUsers()
    {
        Console.Clear();

        string usersDatPath = Path.Combine(gpath, "users.dat");

        if (!File.Exists(usersDatPath) || string.IsNullOrWhiteSpace(File.ReadAllText(usersDatPath)))
        {
            DrawText("No hay usuarios registrados");
            return;
        }

        DrawText("USUARIO", Color.Green, false);
        DrawText(" | ", Color.Gray, false);
        DrawText("FANDOM", Color.White, false);
        DrawText(" | ", Color.Gray, false);
        DrawText("EDAD", Color.Green, false);
        DrawText(" | ", Color.Gray, false);
        DrawText("ROLES BUSCADOS", Color.White, false);
        DrawText(" | ", Color.Gray, false);
        DrawText("PRONOMBRES", Color.Green, false);
        DrawText(" | ", Color.Gray, false);
        DrawText("FECHA REGISTRADA", Color.White, false);
        DrawText(" | ", Color.Gray, false);
        DrawText("RACHA", Color.Green);

        string[] Users = File.ReadAllLines(usersDatPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

        for (int i = 0; i < Users.Length; i++)
        {
            string[] user = Users[i].Split(',');
            string jsonFile = user[1];

            if (!File.Exists(jsonFile)) continue;

            string deserializedUser = File.ReadAllText(jsonFile);

            User readedUser = JsonSerializer.Deserialize<User>(deserializedUser);

            DrawText($"{readedUser.name}", Color.Green, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{readedUser.fandom}", Color.White, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{readedUser.age}", Color.Green, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{string.Join(", ", readedUser.lookedCharacters)}", Color.White, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{string.Join(", ", readedUser.pronauns)}", Color.Green, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{readedUser.dateRegistered}", Color.White, false);
            DrawText(" | ", Color.Gray, false);
            DrawText($"{readedUser.streak}", Color.Green);
        }

    }
    private static void AddUser()
    {
        User newUser = new User();

        Console.Clear();

        newUser.name = TakeInput("Ingresa el nombre: ", Color.Yellow);
        newUser.fandom = TakeInput("Ingresa el fandom: ", Color.Yellow);
        newUser.lookedCharacters = TakeInput("Ingresa los roles buscados (separados por comas): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        newUser.age = Convert.ToInt32(TakeInput("Ingresa la edad: ", Color.Yellow));
        newUser.pronauns = TakeInput("Ingresa los pronombres (separados por comas): ", Color.Yellow).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        string inputStreak = TakeInput("Ingresa la racha (de tenerla): ", Color.Yellow) ?? "0";
        newUser.streak = string.IsNullOrWhiteSpace(inputStreak) ? 0 : Convert.ToInt32(inputStreak);
        newUser.dateRegistered = DateOnly.FromDateTime(DateTime.Now);

        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };

        string UserSerialized = JsonSerializer.Serialize(newUser, options);

        string path = Path.Combine(gpath, $"{newUser.name}.json");

        if (File.Exists(path))
        {
            int count = 1;
            for (int i = 1; File.Exists(Path.Combine(gpath, $"{newUser.name}{i}.json")); i++)
            {
                count = i + 1;
            }
            path = Path.Combine(gpath, $"{newUser.name}{count}.json");
        }

        File.WriteAllText(path, UserSerialized);

        File.AppendAllText(Path.Combine(gpath, "users.dat"), $"{newUser.name},{path}{Environment.NewLine}");
    }
    private static string TakeInput(string prefix = " > ", Color prefixColor = Color.Green, Color textColor = Color.Default)
    {
        ColorSwitch(prefixColor);

        Console.Write(prefix);

        ColorSwitch(textColor);

        string s = Console.ReadLine();

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

        // Leemos solo las líneas válidas
        string[] lines = File.ReadAllLines(usersDatPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

        DrawText("SELECCIONA UN USUARIO:", Color.White);
        DrawText("");

        // Mostramos la lista enumerada usando los nombres de users.dat
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

        // Obtenemos la ruta del JSON del usuario seleccionado (índice base 0)
        string[] selectedUserData = lines[selectedIndex - 1].Split(',');
        string jsonPath = selectedUserData[1];

        if (!File.Exists(jsonPath))
        {
            DrawText($"No se encontro el archivo de datos ({jsonPath}).");
            return;
        }

        // Deserializamos el usuario actual
        string deserializedUser = File.ReadAllText(jsonPath);
        User currentUser = JsonSerializer.Deserialize<User>(deserializedUser);

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

        // Guardamos los cambios actualizados en su archivo .json
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
        DrawText("UserDB v1.0 - Copyright (c) 2026 CMDPlayer216", Color.Gray);

        while (true)
        {
            DrawText("");
            DrawText("1. Mostrar usuarios");
            DrawText("2. Agregar un usuario");
            DrawText("3. Verificar un usuario");
            DrawText("");
            input = TakeInput();

            try
            {
                switch (Convert.ToInt32(input))
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
                    default:
                        DrawText("Esa opcion no existe!", Color.Red);
                        break;
                }
            }
            catch (Exception e)
            {
                DrawText("Esa opcion no existe!", Color.Red);
            }
        }
    }
}