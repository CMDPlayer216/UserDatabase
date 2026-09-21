using System;
using System.Diagnostics;
using System.IO;

namespace userdb.Services;

public class DatabaseLock : IDisposable
{
    private readonly string _lockFilePath;
    private bool _isLocked = false;
    private readonly string logTitle = "DatabaseLock";
    public DatabaseLock(string dbDirectory)
    {
        _lockFilePath = Path.Combine(dbDirectory, "userdb.lock");

        // Suscribirse a las interrupciones del sistema
        Console.CancelKeyPress += OnCancelKeyPress;
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
    }

    public bool Acquire()
    {
        UserService.EnsureDirectoryExists();

        if (File.Exists(_lockFilePath))
        {
            // 1. Verificar si el lock pertenece a ESTE MISMO proceso
            if (IsOwnedByCurrentProcess())
            {
                _isLocked = true;
                return true; // Permitir reentrancia/llamadas anidadas
            }

            // 2. Verificar si es un lock huérfano de una sesión anterior
            if (IsStaleLock())
            {
                Logs.Log(logTitle, "Se detectó un archivo .lock huérfano. Liberando...", Logs.logType.Warning, 2);
                Release();
            }
            else
            {
                return false; // Bloqueado por OTRO proceso distinto
            }
        }

        try
        {
            // Escribir el PID actual en el archivo lock
            File.WriteAllText(_lockFilePath, Environment.ProcessId.ToString());
            _isLocked = true;
            return true;
        }
        catch (Exception ex)
        {
            DrawText($"[Error] No se pudo crear el archivo lock: {ex.Message}", Color.Red);
            Logs.Log(logTitle, "No se pudo crear el archivo lock", Logs.logType.Error, 2);
            return false;
        }
    }

    private bool IsOwnedByCurrentProcess()
    {
        try
        {
            string content = File.ReadAllText(_lockFilePath).Trim();
            if (int.TryParse(content, out int pid))
            {
                return pid == Environment.ProcessId;
            }
        }
        catch { }
        return false;
    }

    public void Release()
    {
        if (_isLocked && File.Exists(_lockFilePath))
        {
            try
            {
                File.Delete(_lockFilePath);
            }
            catch { }
        }
        _isLocked = false;
    }

    private bool IsStaleLock()
    {
        try
        {
            string content = File.ReadAllText(_lockFilePath).Trim();
            if (int.TryParse(content, out int pid))
            {
                // Si el proceso guardado en el .lock NO está corriendo, el lock es huérfano
                Process.GetProcessById(pid);
                return false; // El proceso sigue vivo
            }
        }
        catch (ArgumentException)
        {
            // GetProcessById lanza ArgumentException si el PID ya no existe
            return true; // Lock huérfano
        }
        catch
        {
            // Ante cualquier error de lectura, asumimos que no se puede validar
        }

        return false;
    }

    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        // Ocurre al presionar Ctrl+C o Ctrl+Break
        Release();
    }

    private void OnProcessExit(object? sender, EventArgs e)
    {
        // Ocurre al cerrar la aplicación de forma normal o por señal de salida (SIGTERM)
        Release();
    }

    public void Dispose()
    {
        Release();
        Console.CancelKeyPress -= OnCancelKeyPress;
        AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
    }
}