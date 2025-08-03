using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace tuFactura.utilitarios.Herramientas.Logging
{
    public class FileLoggerService : ILoggerService
    {
        private readonly string _logDirectory;
        private readonly object _lockObject = new object();

        public FileLoggerService()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public void LogDebug(string message, object? context = null)
        {
            WriteLog(LogLevel.Debug, message, null, context);
        }

        public void LogInformation(string message, object? context = null)
        {
            WriteLog(LogLevel.Information, message, null, context);
        }

        public void LogWarning(string message, object? context = null)
        {
            WriteLog(LogLevel.Warning, message, null, context);
        }

        public void LogError(string message, Exception? exception = null, object? context = null)
        {
            WriteLog(LogLevel.Error, message, exception, context);
        }

        public void LogCritical(string message, Exception? exception = null, object? context = null)
        {
            WriteLog(LogLevel.Critical, message, exception, context);
        }

        private void WriteLog(LogLevel level, string message, Exception? exception = null, object? context = null)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                Level = level.ToString(),
                Message = message,
                Exception = exception?.ToString(),
                Context = context
            };

            var logFile = Path.Combine(_logDirectory, $"log_{DateTime.Now:yyyy-MM-dd}.txt");
            var logLine = $"[{logEntry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

            if (exception != null)
            {
                logLine += $"\nException: {exception}";
            }

            if (context != null)
            {
                logLine += $"\nContext: {JsonSerializer.Serialize(context)}";
            }

            logLine += "\n" + new string('-', 80) + "\n";

            lock (_lockObject)
            {
                File.AppendAllText(logFile, logLine);
            }
        }
    }
} 