using Observability.Shared.Contracts;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Shared.DefaultImplementations
{
    public class SerilogStructuredLogger : IStructuredLogger
    {
        private static readonly AsyncLocal<LogContext?> _current = new();
        private readonly ILogger _logger;

        public SerilogStructuredLogger(ILogger logger)
        {
            _logger = logger;
        }

        public LogContext Current => _current.Value ??= new LogContext();

        public void Set(LogContext context)
        {
            _current.Value = context;
        }

        public void LogWarning(string source, string operation, string message, Exception? ex = null) =>
            Log(AppLogLevel.Warning, source, operation, message);

        public void LogError(string source, string operation, string message, Exception? ex = null) =>
            Log(AppLogLevel.Error, source, operation, message, ex);

        public void LogCritical(string source, string operation, string message, Exception? ex = null) =>
            Log(AppLogLevel.Critical, source, operation, message, ex);

        private void Log(AppLogLevel level, string source, string operation, string message, Exception? ex = null)
        {
            var ctx = Current;
            ctx.Source = source;
            ctx.Operation = operation;
            ctx.Message = message;
            ctx.Timestamp = DateTimeOffset.UtcNow;
            ctx.Exception = ex;
            ctx.Level = level;

            _logger
                .ForContext("CorrelationId", ctx.CorrelationId ?? "N/A")
                .ForContext("TraceId", ctx.TraceId ?? "N/A")
                .ForContext("Source", ctx.Source ?? "N/A")
                .ForContext("Operation", ctx.Operation ?? "N/A")
                .ForContext("UserId", ctx.UserId ?? "N/A")
                .ForContext("SessionId", ctx.SessionId ?? "N/A")
                .ForContext("Timestamp", ctx.Timestamp ?? DateTimeOffset.MinValue)
                .ForContext("Level", ctx.Level ?? AppLogLevel.None)
                .Write(MapLevel(level), ex, "{Message}", message);
        }

        private static LogEventLevel MapLevel(AppLogLevel level) => level switch
        {
            AppLogLevel.Warning => LogEventLevel.Warning,
            AppLogLevel.Error => LogEventLevel.Error,
            AppLogLevel.Critical => LogEventLevel.Fatal,
            AppLogLevel.None => LogEventLevel.Verbose,
            _ => LogEventLevel.Information
        };
    }
}
