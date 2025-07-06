using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Shared.Contracts
{
    public interface ITracingService
    {
        void ExtractTraceIdToLogContext(string operationName);
        IDisposable? StartActivity(string operationName);
    }
}
