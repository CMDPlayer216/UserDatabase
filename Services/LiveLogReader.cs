using System;
using System.IO;
using System.Text;
using System.Threading;

namespace userdb.Services;

public class RealTimeLogReader(string filePath) : IDisposable
{
    private readonly string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    private FileSystemWatcher? _watcher;
    private long _lastPosition = 0;
    private bool _isDisposed = false;

    /// <summary>
    /// Inicia el monitoreo del archivo de log.
    /// </summary>
    /// <param name="readExistingContent">
    /// Si es true, lee todo el contenido previo del archivo al iniciar.
    /// Si es false, solo leerá las líneas nuevas a partir de ahora.
    /// </param>
    public void Start(bool readExistingContent = false)
    {
        if (!File.Exists(_filePath))
        {
            string? directoryPath = Path.GetDirectoryName(Path.GetFullPath(_filePath));
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.Create(_filePath).Close();
        }

        FileInfo fileInfo = new FileInfo(_filePath);
        _lastPosition = readExistingContent ? 0 : fileInfo.Length;

        ProcesarNuevasLineas();

        string folder = Path.GetDirectoryName(Path.GetFullPath(_filePath))!;
        string fileName = Path.GetFileName(_filePath);

        _watcher = new FileSystemWatcher(folder, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        _watcher.Changed += OnFileChanged;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        Thread.Sleep(50);
        ProcesarNuevasLineas();
    }

    private void ProcesarNuevasLineas()
    {
        try
        {
            using FileStream stream = new(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (stream.Length < _lastPosition)
            {
                _lastPosition = 0;
                DrawText("--- El archivo fue reiniciado o rotado ---", Color.Yellow, true);
            }

            stream.Seek(_lastPosition, SeekOrigin.Begin);

            using StreamReader reader = new(stream, Encoding.UTF8);
            string? linea;
            while ((linea = reader.ReadLine()) != null)
            {
                ImprimirLineaLog(linea);
            }

            _lastPosition = stream.Position;
        }
        catch (IOException)
        {
            // Reintento silencioso en el siguiente evento si el archivo estaba bloqueado
        }
    }

    private void ImprimirLineaLog(string linea)
    {
        // Mapeo visual utilizando tu método DrawText
        if (linea.Contains("ERROR", StringComparison.OrdinalIgnoreCase))
        {
            DrawText(linea, Color.Red, insertNewLine: true);
        }
        else if (linea.Contains("WARN", StringComparison.OrdinalIgnoreCase) || linea.Contains("WARNING", StringComparison.OrdinalIgnoreCase))
        {
            DrawText(linea, Color.Yellow, insertNewLine: true);
        }
        else
        {
            DrawText(linea, Color.White, insertNewLine: true);
        }
    }

    public void Stop()
    {
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Changed -= OnFileChanged;
            _watcher.Dispose();
            _watcher = null;
        }
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            Stop();
            _isDisposed = true;
        }
    }
}