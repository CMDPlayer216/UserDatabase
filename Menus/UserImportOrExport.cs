using userdb.Commands;
using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus;

public static class UserImportOrExportMenu
{
    private static void ImportUser()
    {
        Logs.Log("UserImportOrExportMenu.ImportUser", "Iniciando importación interactiva de usuario", Logs.logType.Info, 2);
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
                    mode = "combine-keeping-new";
                    break;
                default:
                    mode = "keep";
                    break;
            }
        }
        catch (Exception e)
        {
            Logs.Log("UserImportOrExportMenu.ImportUser", $"Error al seleccionar modo: {e.Message}", Logs.logType.Error, 3);
            DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
        }

        userdb.Commands.ImportUser.Run(source, mode);
    }

    private static void ExportUser()
    {
        Logs.Log("UserImportOrExportMenu.ExportUser", "Iniciando exportación interactiva de usuario", Logs.logType.Info, 2);
        Console.Clear();

        List<string> lines = UserService.GetUserIndexLines();

        if (lines.Count == 0)
        {
            Logs.Log("UserImportOrExportMenu.ExportUser", "No hay usuarios registrados para exportar", Logs.logType.Info, 1);
            DrawText("No hay usuarios registrados para exportar.", Color.Red);
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
            Logs.Log("UserImportOrExportMenu.ExportUser", $"Selección inválida: '{inputSelection}'", Logs.logType.Warning, 2);
            DrawText("Seleccion invalida.", Color.Red);
            return;
        }

        int realindex = selectedIndex - 1;
        List<string> user = lines[realindex].Split(',').ToList();
        string dest = TakeInput("Ruta para exportar: ");

        User? userObj = UserService.LoadUserFromJson(user[1]);

        if (userObj == null)
        {
            Logs.Log("UserImportOrExportMenu.ExportUser", $"Error al cargar usuario desde {user[1]}", Logs.logType.Error, 3);
            DrawText("No se pudo cargar el usuario.", Color.Red);
            return;
        }

        userdb.Commands.ExportUser.Run(userObj.userId, dest);
    }

    public static void Show()
    {
        Logs.Log("UserImportOrExportMenu", "Mostrando menú Importar/Exportar Usuario", Logs.logType.Info, 2);
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
            Logs.Log("UserImportOrExportMenu", $"Error en menú: {e.Message}", Logs.logType.Error, 3);
            DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
        }
    }
}
