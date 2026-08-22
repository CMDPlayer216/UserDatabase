using System.Text.Json;

namespace userdb.Services;

public static class RegenerateIndex
{
    public static void Run(string gPath)
    {
        string logTitle = "RegenerateIndex";
        string datPath = Path.Combine(gPath, "users.dat");

        Logs.Log(logTitle, $"datPath = {datPath}", Logs.logType.Info, 1);

        // 1. Si no existe la carpeta contenedora, no hay nada que escanear
        if (!Directory.Exists(gPath))
        {
            Logs.Log(logTitle, $"El directorio {gPath} no existe, creando uno nuevo", Logs.logType.Warning, 2);
            Directory.CreateDirectory(gPath);
            Logs.Log(logTitle, $"El archivo {datPath} no existe, creando uno nuevo", Logs.logType.Warning, 2);
            if (File.Exists(datPath)) File.Delete(datPath);
            return;
        }

        // 2. Buscamos todos los archivos .json en la carpeta
        List<string> jsonFiles = Directory.GetFiles(gPath, "*.json", SearchOption.TopDirectoryOnly).ToList();

        Logs.Log(logTitle, $"Archivos JSON encontrados en la carpeta: {jsonFiles.Count}", Logs.logType.Info, 1);

        // Si no hay usuarios guardados, aseguramos borrar el .dat viejo si existía
        if (jsonFiles.Count == 0)
        {
            if (File.Exists(datPath))
            {
                File.Delete(datPath);
                Logs.Log(logTitle, $"Eliminando {datPath}", Logs.logType.Warning, 1);
            }
            return;
        }

        List<string> indexLines = new List<string>();

        // 3. Inspeccionamos cada JSON para extraer únicamente 'name' y 'userId'
        foreach (string filePath in jsonFiles)
        {
            try
            {
                using FileStream stream = File.OpenRead(filePath);
                using JsonDocument doc = JsonDocument.Parse(stream);

                JsonElement root = doc.RootElement;

                // Intentamos obtener las propiedades 'name' y 'userId' (o Name / UserId)
                string? name = null;
                string? userId = null;

                if (root.TryGetProperty("name", out JsonElement nameElement) ||
                    root.TryGetProperty("Name", out nameElement))
                {
                    name = nameElement.GetString();
                }

                if (root.TryGetProperty("userId", out JsonElement idElement) ||
                    root.TryGetProperty("UserId", out idElement))
                {
                    userId = idElement.GetString();
                }

                // 4. Si la estructura es válida, construimos la línea con el formato exacto:
                // {name},{GPath + userId}.json
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(userId))
                {
                    string userJsonPath = Path.Combine(gPath, $"{userId}.json");
                    indexLines.Add($"{name},{userJsonPath}");
                }
            }
            catch (JsonException exception)
            {
                Logs.Log(logTitle, exception.Message, Logs.logType.Error, 4);
                // Si hay algún .json corrupto o mal formado en la carpeta, 
                // lo ignoramos para que no rompa el índice de los demás.
                continue;
            }
            catch (Exception exception)
            {
                Logs.Log(logTitle, exception.Message, Logs.logType.Error, 4);
                // Ignorar cualquier otro error de lectura puntual de ese archivo
                continue;
            }
        }

        Logs.Log(logTitle, $"Archivos JSON válidos encontrados: {indexLines.Count}", Logs.logType.Info, 1);

        // 5. Reescribimos el archivo users.dat con las entradas reconstruidas
        if (indexLines.Count > 0)
        {
            File.WriteAllLines(datPath, indexLines);
        }
        else if (File.Exists(datPath))
        {
            File.Delete(datPath);
        }
    }
}
