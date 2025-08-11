using Observability.Shared.Contracts;
using Observability.Shared.Helpers;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Shared.DefaultImplementations
{
    public class OpenTelemetryTracingService : ITracingService
    {
        private readonly ActivitySource _activitySource;

        private readonly IStructuredLogger _logger;

        public OpenTelemetryTracingService(
            string activitySourceName,
            IStructuredLogger structuredLogger)
        {
            _activitySource = new ActivitySource(activitySourceName);
            _logger = structuredLogger;
        }

        public void ExtractTraceIdToLogContext()
        {
            var (source, operation) = CallerInfo.GetCallerClassAndMethod();

            var activity = Activity.Current;

            if (activity != null)
            {
                _logger.Current.TraceId = activity.TraceId.ToString();
                _logger.Current.CorrelationId = _logger.Current.TraceId;
            }
            else
            {
                _logger.LogWarning(
                    source,
                    operation,
                    "No active Activity found. Tracing might not be set up correctly or the request is missing trace headers.");
            }
        }

        public IDisposable? StartActivity(string tracingOperationName)
        {
            var (source, operation) = CallerInfo.GetCallerClassAndMethod();

            var activity = _activitySource.StartActivity(tracingOperationName, ActivityKind.Internal);

            if (activity == null)
            {
                _logger.LogWarning(source, operation, "Activity could not be started — no listener or sampling prevented it.");
            }

            return activity;
        }
    }
}
