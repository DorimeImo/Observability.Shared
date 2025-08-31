using FluentAssertions;
using Moq;
using Observability.Shared.Contracts;
using Observability.Shared.DefaultImplementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Shared.Tests
{
    public class OpenTelemetryTracingServiceTests
    {
        private ActivitySource Source { get; }

        public OpenTelemetryTracingServiceTests()
        {
            ActivitySource.AddActivityListener(new ActivityListener
            {
                ShouldListenTo = s => s.Name == "TestSource",
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStarted = _ => { },
                ActivityStopped = _ => { }
            });

            Source = new ActivitySource("TestSource");
        }

        // -----------------------------------------
        // ✅ Core success and expected flow tests
        // -----------------------------------------

        [Fact]
        public void StartActivity_ShouldStartActivity_WhenCalled()
        {
            // Arrange
            var loggerMock = new Mock<IStructuredLogger>();
            var service = new OpenTelemetryTracingService(Source, loggerMock.Object);

            // Act
            using var activity = service.StartActivity("TestOperation");

            // Assert
            activity.Should().NotBeNull();
            Activity.Current.Should().NotBeNull();
            Activity.Current!.OperationName.Should().Be("TestOperation");
        }

        [Fact]
        public void ExtractTraceIdToLogContext_ShouldSetTraceId_WhenActivityIsActive()
        {
            // Arrange
            var loggerMock = new Mock<IStructuredLogger>();
            loggerMock.Setup(l => l.Current).Returns(new LogContext());

            var service = new OpenTelemetryTracingService(Source, loggerMock.Object);

            using var activity = new ActivitySource("TestSource").StartActivity("TestOperation");

            // Act
            service.CorrelateActivityAndLogger();

            // Assert
            loggerMock.VerifyGet(l => l.Current, Times.AtLeastOnce());
            loggerMock.Object.Current.TraceId.Should().Be(activity!.TraceId.ToString());

            activity.Stop();
        }

        // ----------------------------------------------
        // ❌ Exception handling and logging verification
        // ----------------------------------------------

        [Fact]
        public void ExtractTraceIdToLogContext_ShouldLogWarning_WhenNoActivity()
        {
            // Arrange
            var loggerMock = new Mock<IStructuredLogger>();
            var service = new OpenTelemetryTracingService(Source, loggerMock.Object);

            // Act
            service.CorrelateActivityAndLogger();

            // Assert
            loggerMock.Verify(l =>
                l.LogWarning(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<string>(msg => msg != null && msg.Contains("No active Activity found")),
                    It.IsAny<Exception?>()),
                Times.Once);
        }

        [Fact]
        public void StartActivity_ShouldLogWarning_WhenActivityIsNull()
        {
            // Arrange
            using var unobserved = new ActivitySource("UnobservedSource");
            var loggerMock = new Mock<IStructuredLogger>();
            var service = new OpenTelemetryTracingService(unobserved, loggerMock.Object);

            // Act
            using var activity = service.StartActivity("TestOperation");

            // Assert
            activity.Should().BeNull();
            loggerMock.Verify(l =>
                l.LogWarning(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<string>(msg => msg.Contains("Activity could not be started")),
                    It.IsAny<Exception?>()),
                Times.Once);
        }
    }
}
