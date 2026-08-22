using System.IO.Compression;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Commands;

public static class ExportDataBase
{
    public static void Run(string? path)
    {
        string logTitle = "ExportDataBase";
        Logs.Log(logTitle, $"Iniciando exportación de base de datos a ruta solicitada: '{path}'", Logs.logType.Info, 2);

        if (string.IsNullOrWhiteSpace(path))
        {
            string fecha = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            path = $"User-Data-Base-Exported-{fecha}.userdb";
        }
        else if (!path.EndsWith(".userdb", StringComparison.OrdinalIgnoreCase))
        {
            path += ".userdb";
        }

        List<string> dataBase = Directory.GetFiles(UserService.GPath).ToList();
        Logs.Log(logTitle, $"Comprimiendo {dataBase.Count} archivos en {path}", Logs.logType.Info, 2);

        using (ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Create))
        {
            foreach (string filePath in dataBase)
            {
                string fileName = Path.GetFileName(filePath);
                zip.CreateEntryFromFile(filePath, fileName, CompressionLevel.Optimal);
            }
        }

        Logs.Log(logTitle, $"Base de datos exportada con éxito a {path}", Logs.logType.Info, 2);
        DrawText($"Base de datos exportada con éxito a: {path}", Color.Green);
    }
}