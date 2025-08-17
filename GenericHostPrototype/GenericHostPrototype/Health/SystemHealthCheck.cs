using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

/// <summary>
/// Health check that reports on system memory usage and uptime, configurable via options.
/// </summary>
public class SystemHealthCheck(
    IOptions<SystemHealthOptions> options, 
    ILogger<SystemHealthCheck> logger) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check system metrics
            var memoryUsage = GC.GetTotalMemory(false);
            var processorCount = Environment.ProcessorCount;
            var uptimeMinutes = (DateTime.Now - Process.GetCurrentProcess().StartTime).TotalMinutes;

            var isHealthy = memoryUsage < options.Value.MaxMemoryThreshold &&
                          uptimeMinutes < options.Value.MaxUptimeMinutes;

            var data = new Dictionary<string, object>
            {
                { "MemoryUsage", memoryUsage },
                { "ProcessorCount", processorCount },
                { "UptimeMinutes", uptimeMinutes }
            };

            if (isHealthy)
            {
                logger.LogInformation("System is healthy. Memory: {Memory} bytes", memoryUsage);
                return Task.FromResult(HealthCheckResult.Healthy("System is healthy", data));
            }

            logger.LogWarning("System health check failed. Memory: {Memory} bytes", memoryUsage);
            return Task.FromResult(HealthCheckResult.Unhealthy("System exceeds health thresholds", null, data));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error performing health check");
            return Task.FromResult(HealthCheckResult.Unhealthy("Health check failed", ex));
        }
    }
}