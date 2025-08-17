public class BackgroundQueueTests
{
    [Fact]
    public async Task EnqueueAndDequeue_Works()
    {
        var queue = new BackgroundTaskQueue(10);
        bool executed = false;
        await queue.QueueWorkItemAsync(async token => { executed = true; await Task.CompletedTask; });
        var workItem = await queue.DequeueAsync(CancellationToken.None);
        await workItem(CancellationToken.None);
        Assert.True(executed);
    }

    [Fact]
    public async Task QueueWorkItemAsync_ThrowsOnNull()
    {
        var queue = new BackgroundTaskQueue(10);
        // Convert ValueTask to Task for Assert.ThrowsAsync
        await Assert.ThrowsAsync<ArgumentNullException>(() => queue.QueueWorkItemAsync(null!).AsTask());
    }

    [Fact]
    public async Task Queue_IsBounded()
    {
        var queue = new BackgroundTaskQueue(1);
        await queue.QueueWorkItemAsync(async token => await Task.CompletedTask);
        var enqueueTask = queue.QueueWorkItemAsync(async token => await Task.CompletedTask);
        Assert.False(enqueueTask.IsCompleted); // Should block until dequeued
        var workItem = await queue.DequeueAsync(CancellationToken.None);
        await workItem(CancellationToken.None);
        await enqueueTask; // Should now complete
    }
}
