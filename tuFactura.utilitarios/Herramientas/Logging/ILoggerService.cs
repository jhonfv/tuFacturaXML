using System;

namespace tuFactura.utilitarios.Herramientas.Logging
{
    public enum LogLevel
    {
        Debug,
        Information,
        Warning,
        Error,
        Critical
    }

    public interface ILoggerService
    {
        void LogDebug(string message, object context = null);
        void LogInformation(string message, object context = null);
        void LogWarning(string message, object context = null);
        void LogError(string message, Exception exception = null, object context = null);
        void LogCritical(string message, Exception exception = null, object context = null);
    }
} 