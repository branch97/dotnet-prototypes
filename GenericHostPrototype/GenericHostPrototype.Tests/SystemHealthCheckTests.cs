using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

public class SystemHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthy_WhenWithinThresholds()
    {
        var options = Options.Create(new SystemHealthOptions
        {
            MaxMemoryThreshold = long.MaxValue,
            MaxUptimeMinutes = double.MaxValue
        });
        var loggerMock = new Mock<ILogger<SystemHealthCheck>>();
        var check = new SystemHealthCheck(options, loggerMock.Object);
        var result = await check.CheckHealthAsync(new HealthCheckContext());
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsUnhealthy_WhenOverThresholds()
    {
        var options = Options.Create(new SystemHealthOptions
        {
            MaxMemoryThreshold = 0,
            MaxUptimeMinutes = 0
        });
        var loggerMock = new Mock<ILogger<SystemHealthCheck>>();
        var check = new SystemHealthCheck(options, loggerMock.Object);
        var result = await check.CheckHealthAsync(new HealthCheckContext());
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }
}
