Shared observability contracts and default implementations for .NET microservices.

---

## ✨ Features

- `IStructuredLogger`: Structured logging abstraction
- `ITracingService`: Tracing abstraction
- `LogContext`: Consistent contextual logging (trace ID, correlation ID, etc.)
- `AppLogLevel`: Unified log level enumeration
- `CallerInfo`: Helper to auto-capture source and operation names
- `SerilogStructuredLogger`: **Implemented primarily for exception and error logging, not for normal application flow logs**
- `OpenTelemetryTracingService`: OpenTelemetry-based tracing implementation

---

## 🚀 Getting Started

### Installation

```bash
dotnet add package DorimeImo.Observability.Shared

## 📦 Dependencies

This package references:

- [Serilog](https://www.nuget.org/packages/Serilog) (Apache-2.0)
- [OpenTelemetry](https://www.nuget.org/packages/OpenTelemetry) (Apache-2.0)
- [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging) (MIT)

No modifications have been made to these dependencies.