using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Shared.Contracts
{
    public interface IStructuredLogger
    {
        LogContext Current { get; }
        void Set(LogContext context);
        void LogWarning(string source, string operation, string message, Exception? ex = null);
        void LogError(string source, string operation, string message, Exception? ex = null);
        void LogCritical(string source, string operation, string message, Exception? ex = null);
    }
}
