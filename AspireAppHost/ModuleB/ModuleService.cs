using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ModuleB;

public class ModuleService(ILogger<ModuleService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Module B is running at: {time}", DateTimeOffset.Now);
            await Task.Delay(5000, stoppingToken);
        }
    }
}
