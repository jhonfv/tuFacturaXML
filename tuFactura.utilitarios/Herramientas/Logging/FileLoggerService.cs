using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace tuFactura.utilitarios.Herramientas.Logging
{
    public class FileLoggerService : ILoggerService
    {
        private readonly string _logFilePath;
        private readonly object _lockObject = new object();

        public FileLoggerService(string logDirectory = null)
        {
            // Si no se especifica directorio, usar la ruta del webApp
            var directory = logDirectory ?? GetWebAppLogDirectory();
            Directory.CreateDirectory(directory);
            
            var fileName = $"tuFacturaXML_{DateTime.Now:yyyyMMdd}.log";
            _logFilePath = Path.Combine(directory, fileName);
        }

        private string GetWebAppLogDirectory()
        {
            // Intentar encontrar la ruta del webApp
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // Si estamos en el directorio del webApp, usar Logs ahí
            if (currentDir.Contains("tuFacturaXML.webApp"))
            {
                return Path.Combine(currentDir, "Logs");
            }
            
            // Si no, buscar hacia arriba en la estructura de directorios
            var directory = new DirectoryInfo(currentDir);
            while (directory != null && !directory.Name.Contains("tuFacturaXML.webApp"))
            {
                directory = directory.Parent;
            }
            
            if (directory != null)
            {
                return Path.Combine(directory.FullName, "Logs");
            }
            
            // Fallback: usar el directorio actual
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        }

        public void LogDebug(string message, object context = null)
        {
            WriteLog(LogLevel.Debug, message, null, context);
        }

        public void LogInformation(string message, object context = null)
        {
            WriteLog(LogLevel.Information, message, null, context);
        }

        public void LogWarning(string message, object context = null)
        {
            WriteLog(LogLevel.Warning, message, null, context);
        }

        public void LogError(string message, Exception exception = null, object context = null)
        {
            WriteLog(LogLevel.Error, message, exception, context);
        }

        public void LogCritical(string message, Exception exception = null, object context = null)
        {
            WriteLog(LogLevel.Critical, message, exception, context);
        }

        private void WriteLog(LogLevel level, string message, Exception exception = null, object context = null)
        {
            var logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                Exception = exception?.ToString(),
                StackTrace = exception?.StackTrace,
                Context = context != null ? JsonSerializer.Serialize(context, new JsonSerializerOptions { WriteIndented = true }) : null
            };

            var logLine = FormatLogEntry(logEntry);

            lock (_lockObject)
            {
                try
                {
                    File.AppendAllText(_logFilePath, logLine + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    // Fallback: escribir en consola si no se puede escribir en archivo
                    Console.WriteLine($"Error writing to log file: {ex.Message}");
                    Console.WriteLine($"Original log entry: {logLine}");
                }
            }
        }

        private string FormatLogEntry(LogEntry entry)
        {
            var levelColor = GetLevelColor(entry.Level);
            var timestamp = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
            
            var logLine = $"[{timestamp}] [{levelColor}{entry.Level,-8}\x1b[0m] {entry.Message}";
            
            if (!string.IsNullOrEmpty(entry.Exception))
            {
                logLine += $"\nException: {entry.Exception}";
            }
            
            if (!string.IsNullOrEmpty(entry.StackTrace))
            {
                logLine += $"\nStackTrace: {entry.StackTrace}";
            }
            
            if (!string.IsNullOrEmpty(entry.Context))
            {
                logLine += $"\nContext: {entry.Context}";
            }

            return logLine;
        }

        private string GetLevelColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => "\x1b[36m",     // Cyan
                LogLevel.Information => "\x1b[32m", // Green
                LogLevel.Warning => "\x1b[33m",     // Yellow
                LogLevel.Error => "\x1b[31m",       // Red
                LogLevel.Critical => "\x1b[35m",    // Magenta
                _ => "\x1b[0m"                      // Reset
            };
        }

        private class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public LogLevel Level { get; set; }
            public string Message { get; set; }
            public string Exception { get; set; }
            public string StackTrace { get; set; }
            public string Context { get; set; }
        }
    }
} 