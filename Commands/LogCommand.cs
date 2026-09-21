using userdb.Services;
namespace userdb.Commands;

public static class LogCommand
{
    public static void Run(bool isLiveMode, int logAmmount)
    {
        string logPath = Services.Logs.GetCurrentLogPath();
        if (logAmmount == -1) logAmmount = int.MaxValue;
        if (isLiveMode)
        {
            using var reader = new RealTimeLogReader(logPath);
            // Pasar 'false' para leer solo lo NUEVO a partir de este momento.
            // Pasar 'true' si quieres que imprima también lo que ya estaba escrito en el archivo.
            reader.Start(readExistingContent: false);

            Console.WriteLine("Escuchando logs en tiempo real... Presiona ENTER para detener.");
            Console.ReadLine();

            reader.Stop();
            return;
        }

        string[] lines = [.. File.ReadAllLines(logPath).Reverse()];

        for (int i = 0; i < lines.Length && i <= logAmmount; i++)
        {
            Color color = Color.Default;
            string line = lines[i];
            if (line.Contains("ERROR")) color = Color.Red;
            else if (line.Contains("WARN")) color = Color.Yellow;
            else color = Color.White;
            DrawText(lines[i], color);
        }
    }
}
