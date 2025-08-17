/// <summary>
/// Interface for a background task queue that allows enqueuing and dequeuing work items.
/// </summary>
public interface IBackgroundTaskQueue
{
    /// <summary>
    /// Enqueue a work item for background processing.
    /// </summary>
    ValueTask QueueWorkItemAsync(Func<CancellationToken, ValueTask> workItem);

    /// <summary>
    /// Dequeue a work item for processing.
    /// </summary>
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}
