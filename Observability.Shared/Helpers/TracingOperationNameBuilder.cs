using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Shared.Helpers
{
    public static class TracingOperationNameBuilder
    {
        public static string TracingOperationNameBuild((string Source, string Operation) callerInfo)
            => $"{callerInfo.Source}.{callerInfo.Operation}";
    }
}
