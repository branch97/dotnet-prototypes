using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Background service that processes work items from the IBackgroundTaskQueue.
/// </summary>
public class QueuedHostedService(
    IBackgroundTaskQueue taskQueue,
    ILogger<QueuedHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Queued Hosted Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await taskQueue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing task work item.");
            }
        }

        logger.LogInformation("Queued Hosted Service is stopping.");
    }
}
