using Moq;
using Observability.Shared.Contracts;
using Observability.Shared.DefaultImplementations;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Shared.Tests
{
    public class SerilogStructuredLoggerTests
    {
        [Theory]
        [InlineData(AppLogLevel.Warning, LogEventLevel.Warning)]
        [InlineData(AppLogLevel.Error, LogEventLevel.Error)]
        [InlineData(AppLogLevel.Critical, LogEventLevel.Fatal)]
        public void Log_ShouldMapLevelsCorrectly(AppLogLevel appLevel, LogEventLevel expectedSerilogLevel)
        {
            // Arrange
            var serilogMock = new Mock<ILogger>();
            serilogMock
                .Setup(l => l.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                .Returns(serilogMock.Object);
            var logger = new SerilogStructuredLogger(serilogMock.Object);

            // Act
            switch (appLevel)
            {
                case AppLogLevel.Warning:
                    logger.LogWarning("Source", "Op", "Test message");
                    break;
                case AppLogLevel.Error:
                    logger.LogError("Source", "Op", "Test message");
                    break;
                case AppLogLevel.Critical:
                    logger.LogCritical("Source", "Op", "Test message");
                    break;
            }

            // Assert
            serilogMock.Verify(l =>
                l.Write<string>(
                    expectedSerilogLevel,
                    It.IsAny<Exception>(),
                    "{Message}",
                    "Test message"),
                Times.Once);
        }

        [Fact]
        public void Log_ShouldIncludeAllContextProperties()
        {
            // Arrange
            var serilogMock = new Mock<ILogger>();
            serilogMock.Setup(l => l.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                       .Returns(serilogMock.Object);

            var logger = new SerilogStructuredLogger(serilogMock.Object);

            var context = new LogContext
            {
                CorrelationId = "corr-id",
                TraceId = "trace-id",
                UserId = "user-id",
                SessionId = "session-id"
            };
            logger.Set(context);

            // Act
            logger.LogError("SourceClass", "OperationName", "Test error message", null);

            // Assert: Verify ForContext calls
            serilogMock.Verify(l => l.ForContext("CorrelationId", "corr-id", false), Times.Once);
            serilogMock.Verify(l => l.ForContext("TraceId", "trace-id", false), Times.Once);
            serilogMock.Verify(l => l.ForContext("UserId", "user-id", false), Times.Once);
            serilogMock.Verify(l => l.ForContext("SessionId", "session-id", false), Times.Once);
            serilogMock.Verify(l =>
                l.Write<string>(
                    It.IsAny<LogEventLevel>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }
    }
}
