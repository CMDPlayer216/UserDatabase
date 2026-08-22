using userdb.Commands;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus;

public static class DataBaseImportOrExportMenu
{
    private static void ImportDataBase()
    {
        Logs.Log("DataBaseImportOrExportMenu.ImportDataBase", "Iniciando importación interactiva de base de datos", Logs.logType.Info, 2);
        Console.Clear();
        string source = TakeInput("Ruta del archivo .userdb: ");
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
            Logs.Log("DataBaseImportOrExportMenu.ImportDataBase", $"Error al seleccionar modo: {e.Message}", Logs.logType.Error, 3);
            DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
        }

        if (!source.EndsWith(".userdb", StringComparison.OrdinalIgnoreCase)) source += ".userdb";

        userdb.Commands.ImportDataBase.Run(source, mode);
    }

    private static void ExportDataBase()
    {
        Logs.Log("DataBaseImportOrExportMenu.ExportDataBase", "Iniciando exportación interactiva de base de datos", Logs.logType.Info, 2);
        Console.Clear();
        string dest = TakeInput("Archivo de destino: ");
        userdb.Commands.ExportDataBase.Run(dest);
    }

    public static void Show()
    {
        Logs.Log("DataBaseImportOrExportMenu", "Mostrando menú Importar/Exportar Base de Datos", Logs.logType.Info, 2);
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
            Logs.Log("DataBaseImportOrExportMenu", $"Error en menú: {e.Message}", Logs.logType.Error, 3);
            DrawText($"[ERROR - {e.GetType().Name}]: {e.Message}", Color.Red);
        }
    }
}
