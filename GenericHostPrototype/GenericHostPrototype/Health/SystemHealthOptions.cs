/// <summary>
/// Options for configuring the system health check.
/// </summary>
public class SystemHealthOptions
{
    /// <summary>
    /// Maximum allowed memory usage in bytes.
    /// </summary>
    public long MaxMemoryThreshold { get; set; } = 1024L * 1024L * 1024L; // 1 GB
    /// <summary>
    /// Maximum allowed uptime in minutes.
    /// </summary>
    public double MaxUptimeMinutes { get; set; } = 60 * 24; // 24 hours
}
