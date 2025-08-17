using Microsoft.Extensions.Logging;
using Moq;

public class QueuedHostedServiceTests
{
    [Fact]
    public async Task ExecuteAsync_ProcessesWorkItem()
    {
        var loggerMock = new Mock<ILogger<QueuedHostedService>>();
        var queue = new BackgroundTaskQueue(10);
        bool executed = false;
        await queue.QueueWorkItemAsync(async token => { executed = true; await Task.CompletedTask; });
        var service = new QueuedHostedService(queue, loggerMock.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100); // Stop after a short time
        await service.StartAsync(cts.Token);
        Assert.True(executed);
    }

    [Fact]
    public async Task ExecuteAsync_LogsErrorOnException()
    {
        var loggerMock = new Mock<ILogger<QueuedHostedService>>();
        var queue = new BackgroundTaskQueue(10);
        await queue.QueueWorkItemAsync(token => throw new InvalidOperationException("fail"));
        var service = new QueuedHostedService(queue, loggerMock.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100);
        await service.StartAsync(cts.Token);
        loggerMock.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred executing task work item.")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }
}
