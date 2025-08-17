using Microsoft.Extensions.Logging;
using Moq;

namespace GenericHostPrototype.Tests
{
    public class HostedLifecycleServiceTests
    {
        [Fact]
        public async Task HostedLifecycleService_LifecycleMethods_LogInformation()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HostedLifecycleService>>();
            var appLifetimeMock = new Mock<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();
            var service = new HostedLifecycleService(loggerMock.Object, appLifetimeMock.Object);
            var token = CancellationToken.None;

            // Act
            await ((Microsoft.Extensions.Hosting.IHostedLifecycleService)service).StartingAsync(token);
            await ((Microsoft.Extensions.Hosting.IHostedService)service).StartAsync(token);
            await ((Microsoft.Extensions.Hosting.IHostedLifecycleService)service).StartedAsync(token);
            await ((Microsoft.Extensions.Hosting.IHostedLifecycleService)service).StoppingAsync(token);
            await ((Microsoft.Extensions.Hosting.IHostedService)service).StopAsync(token);
            await ((Microsoft.Extensions.Hosting.IHostedLifecycleService)service).StoppedAsync(token);

            // Assert
            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("StartingAsync")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("StartAsync")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("StartedAsync")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("StoppingAsync")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("StopAsync")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("StoppedAsync")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }
    }
}
