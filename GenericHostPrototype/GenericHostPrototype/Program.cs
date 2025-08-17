using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// ------------------------------------------------------------
// GenericHostPrototype: A .NET 10 Console App Demoing Generic Host
// ------------------------------------------------------------
// Features:
// - Dependency Injection
// - Logging
// - Configuration Providers (JSON, env, user secrets)
// - App Lifecycle & Hosted Services
// - Health Checks
// - Background Task Queue
// - Environment-specific config & behavior
// ------------------------------------------------------------

var builder = Host.CreateApplicationBuilder(args);

// -------------------------
// Configure Environment
// -------------------------
// Note: Environment is automatically set from DOTNET_ENVIRONMENT variable
// or defaults to "Production" if not specified
builder.Environment.ApplicationName = "GenericHostPrototype";

// -------------------------
// Add Configuration Sources
// -------------------------
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true)
    // Environment-specific config file
    .AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    // User secrets in Development only
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// -------------------------
// Configure Environment-Specific Services
// -------------------------
if (builder.Environment.IsDevelopment())
{
    // Development-only services
    builder.Logging
        .AddDebug()
        .SetMinimumLevel(LogLevel.Debug);
}
else
{
    // Production/Staging logging
    builder.Logging
        .SetMinimumLevel(LogLevel.Information)
        .AddFilter("Microsoft", LogLevel.Warning);
}

// -------------------------
// Configure Logging (Common)
// -------------------------
builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

// -------------------------
// Configure Host Options
// -------------------------
builder.Services.Configure<HostOptions>(builder.Configuration.GetSection("HostOptions"));
builder.Services.Configure<SystemHealthOptions>(builder.Configuration.GetSection("SystemHealth"));

// -------------------------
// Register Dependencies and Services
// -------------------------
builder.Services
    .AddSingleton<IBackgroundTaskQueue>(_ => new BackgroundTaskQueue(
        // Larger queue size in production
        builder.Environment.IsProduction() ? 1000 : 100
    ))
    .AddHostedService<HostedLifecycleService>()
    .AddHostedService<QueuedHostedService>();

// -------------------------
// Add Health Checks
// -------------------------
var healthChecks = builder.Services.AddHealthChecks();

// Add system health check with environment-specific thresholds
if (builder.Environment.IsDevelopment())
{
    healthChecks.AddCheck<SystemHealthCheck>("system_health_check",
        tags: new[] { "ready" },
        timeout: TimeSpan.FromSeconds(3));
}
else
{
    healthChecks.AddCheck<SystemHealthCheck>("system_health_check",
        tags: new[] { "ready" },
        timeout: TimeSpan.FromSeconds(1));
}

// -------------------------
// Build and Run Host
// -------------------------
var host = builder.Build();

// Queue example work (only in Development)
if (builder.Environment.IsDevelopment())
{
    var queue = host.Services.GetRequiredService<IBackgroundTaskQueue>();
    var logger = host.Services.GetRequiredService<ILogger<Program>>();

    // Example: Call health check manually in development
    var healthCheckService = host.Services.GetRequiredService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>();
    var healthReport = await healthCheckService.CheckHealthAsync();
    logger.LogInformation("Manual health check status: {Status}", healthReport.Status);
    foreach (var entry in healthReport.Entries)
    {
        logger.LogInformation("Health check '{Name}': {Status}", entry.Key, entry.Value.Status);
    }

    _ = Task.Run(async () =>
    {
        await Task.Delay(2000);
        await queue.QueueWorkItemAsync(async token =>
        {
            logger.LogInformation("Starting background work item");
            await Task.Delay(5000, token);
            logger.LogInformation("Completed background work item");
        });
    });
}

try
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Starting application in {Environment} environment",
        builder.Environment.EnvironmentName);
    
    await host.RunAsync();
}
catch (Exception ex)
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Host terminated unexpectedly");
    throw;
}
