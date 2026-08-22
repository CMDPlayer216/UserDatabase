using System.IO.Compression;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Commands;

public static class ImportDataBase
{
    public static void Run(string source, string mode)
    {
        string logTitle = "ImportDataBase";
        Logs.Log(logTitle, $"Iniciando importación de base de datos desde '{source}' en modo '{mode}'", Logs.logType.Info, 2);

        // Paso 1: Validar que el archivo existe
        if (!File.Exists(source))
        {
            Logs.Log(logTitle, $"Archivo no encontrado: {source}", Logs.logType.Error, 3);
            DrawText("El archivo de base de datos especificado no existe!", Color.Red);
            return;
        }

        // Paso 2: Validar la extensión del archivo (.userdb)
        if (!source.EndsWith(".userdb", StringComparison.OrdinalIgnoreCase))
        {
            Logs.Log(logTitle, $"Extensión no válida: {source}", Logs.logType.Warning, 2);
            DrawText("Formato de archivo no compatible. Debe tener extensión .userdb", Color.Red);
            return;
        }

        // Paso 3: Crear una carpeta temporal única para extraer los datos
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"userdb_import_{Guid.NewGuid()}");

        try
        {
            // Paso 4: Descomprimir el archivo zip (.userdb) en el directorio temporal
            Logs.Log(logTitle, $"Descomprimiendo en carpeta temporal: {tempDirectory}", Logs.logType.Info, 1);
            ZipFile.ExtractToDirectory(source, tempDirectory);

            // Paso 5: Buscar todos los archivos JSON de usuarios dentro del paquete descomprimido
            List<string> userFiles = Directory.GetFiles(tempDirectory, "*.json", SearchOption.AllDirectories).ToList();

            if (userFiles.Count == 0)
            {
                Logs.Log(logTitle, "No se encontraron archivos JSON válidos en el archivo .userdb", Logs.logType.Warning, 2);
                DrawText("El archivo .userdb no contiene datos de usuarios válidos.", Color.Yellow);
                return;
            }

            // Paso 6: Procesar cada usuario utilizando la lógica de importación existente
            int importedCount = 0;

            foreach (string userFile in userFiles)
            {
                // Usamos la rutina de carga e importación para cada archivo individual
                ImportUser.Run(userFile, mode);
                importedCount++;
            }

            Logs.Log(logTitle, $"Importación de base de datos completada: {importedCount} usuarios procesados", Logs.logType.Info, 2);
            DrawText($"Proceso finalizado. Se procesaron {importedCount} usuarios en modo '{mode}'.", Color.Green);
        }
        catch (InvalidDataException ex)
        {
            Logs.Log(logTitle, $"Archivo zip/userdb dañado: {ex.Message}", Logs.logType.Error, 3);
            DrawText("El archivo .userdb está dañado o no es un archivo zip válido.", Color.Red);
        }
        catch (Exception ex)
        {
            Logs.Log(logTitle, $"Error al importar base de datos: {ex.Message}", Logs.logType.Error, 3);
            DrawText($"Ocurrió un error al importar la base de datos: {ex.Message}", Color.Red);
        }
        finally
        {
            // Paso 7: Limpiar la carpeta temporal pase lo que pase
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, recursive: true);
                Logs.Log(logTitle, $"Directorio temporal eliminado: {tempDirectory}", Logs.logType.Info, 1);
            }
        }
    }
}