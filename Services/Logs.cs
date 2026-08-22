namespace userdb.Services;

public static class Logs
{
    public enum logType
    {
        Warning,
        Error,
        Info
    }

    public static string LogDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "userdb");

    public static string GetCurrentLogPath()
    {
        return Path.Combine(LogDirectory, $"LOG-{DateTime.Now:dd-MM-yyyy}.log");
    }

    public static void Log(string logTitle, string message, logType type, int priority = 1)
    {
        try
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            string logPath = GetCurrentLogPath();

            if (!File.Exists(logPath))
            {
                File.WriteAllText(logPath, $"---- LOGFILE [{DateTime.Now:dd-MM-yyyy}] ----\n");
            }

            string prefix = type switch
            {
                logType.Warning => "Warning",
                logType.Error => "Error",
                logType.Info => "Info",
                _ => "Info"
            };

            string entry = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] [{prefix}] <{priority} {logTitle}> {message}\n";
            File.AppendAllText(logPath, entry);
        }
        catch
        {
            // Evitar que un fallo al registrar log detenga la aplicación
        }
    }
}
