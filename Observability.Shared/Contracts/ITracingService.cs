using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Shared.Contracts
{
    public interface ITracingService
    {
        void CorrelateActivityAndLogger();
        IDisposable? StartActivity(string operationName);
    }
}
