using Falcon.Domain.Enumerations;

namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a monitored Windows server along with its dependent resources.
/// </summary>
public sealed class Server(
    Guid id,
    string hostname,
    EnvironmentType environment,
    ServerStatus status)
{
    private readonly List<MonitoredService> services = [];
    private readonly List<ScheduledTask> scheduledTasks = [];
    private readonly List<AppPool> appPools = [];
    private readonly List<IisSite> iisSites = [];
    private readonly List<LogFile> logFiles = [];
    private readonly List<MetricPoint> metricPoints = [];

    public Guid Id { get; } = id;

    public string Hostname { get; private set; } = hostname;

    public string? DisplayName { get; private set; }

    public EnvironmentType Environment { get; private set; } = environment;

    public string? IpAddress { get; private set; }

    public string? OperatingSystem { get; private set; }

    public DateTimeOffset? LastHeartbeat { get; private set; }

    public ServerStatus Status { get; private set; } = status;

    public double? CpuPercent { get; private set; }

    public double? MemoryPercent { get; private set; }

    public IReadOnlyCollection<string> Tags => tags.AsReadOnly();

    public IReadOnlyCollection<MonitoredService> Services => services.AsReadOnly();

    public IReadOnlyCollection<ScheduledTask> ScheduledTasks => scheduledTasks.AsReadOnly();

    public IReadOnlyCollection<AppPool> AppPools => appPools.AsReadOnly();

    public IReadOnlyCollection<IisSite> IisSites => iisSites.AsReadOnly();

    public IReadOnlyCollection<LogFile> LogFiles => logFiles.AsReadOnly();

    public IReadOnlyCollection<MetricPoint> MetricPoints => metricPoints.AsReadOnly();

    private readonly List<string> tags = [];

    /// <summary>
    /// Updates metadata fields describing the server.
    /// </summary>
    /// <param name="displayName">Friendly display name.</param>
    /// <param name="environment">Operational environment.</param>
    /// <param name="ipAddress">Primary IP address.</param>
    /// <param name="operatingSystem">Operating system caption.</param>
    /// <param name="tagsToApply">Tags to merge into the current set.</param>
    public void UpdateMetadata(
        string? displayName,
        EnvironmentType environment,
        string? ipAddress,
        string? operatingSystem,
        IEnumerable<string>? tagsToApply)
    {
        DisplayName = displayName;
        Environment = environment;
        IpAddress = ipAddress;
        OperatingSystem = operatingSystem;
        if (tagsToApply is not null)
        {
            foreach (var tag in tagsToApply)
            {
                if (!tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                {
                    tags.Add(tag);
                }
            }
        }
    }

    /// <summary>
    /// Applies heartbeat data to the server aggregate.
    /// </summary>
    /// <param name="status">New status reported by the collector.</param>
    /// <param name="lastHeartbeat">Timestamp of the last heartbeat.</param>
    /// <param name="cpuPercent">CPU utilization percentage.</param>
    /// <param name="memoryPercent">Memory utilization percentage.</param>
    public void ApplyHeartbeat(
        ServerStatus status,
        DateTimeOffset? lastHeartbeat,
        double? cpuPercent,
        double? memoryPercent)
    {
        Status = status;
        LastHeartbeat = lastHeartbeat;
        CpuPercent = cpuPercent;
        MemoryPercent = memoryPercent;
    }

    /// <summary>
    /// Registers a monitored service for this server.
    /// </summary>
    /// <param name="service">Service to register.</param>
    public void AddService(MonitoredService service)
    {
        services.RemoveAll(s => s.Id == service.Id);
        services.Add(service);
    }

    /// <summary>
    /// Removes a monitored service by identifier.
    /// </summary>
    /// <param name="serviceId">Unique identifier of the service.</param>
    public void RemoveService(Guid serviceId)
    {
        services.RemoveAll(s => s.Id == serviceId);
    }

    /// <summary>
    /// Adds or replaces a scheduled task associated with this server.
    /// </summary>
    /// <param name="task">Scheduled task to add.</param>
    public void AddScheduledTask(ScheduledTask task)
    {
        scheduledTasks.RemoveAll(t => t.Id == task.Id);
        scheduledTasks.Add(task);
    }

    /// <summary>
    /// Removes a scheduled task from the server.
    /// </summary>
    /// <param name="taskId">Identifier of the task to remove.</param>
    public void RemoveScheduledTask(Guid taskId)
    {
        scheduledTasks.RemoveAll(t => t.Id == taskId);
    }

    /// <summary>
    /// Adds an IIS application pool to the server inventory.
    /// </summary>
    /// <param name="pool">Application pool instance.</param>
    public void AddAppPool(AppPool pool)
    {
        appPools.RemoveAll(p => p.Id == pool.Id);
        appPools.Add(pool);
    }

    /// <summary>
    /// Adds an IIS site to the server inventory.
    /// </summary>
    /// <param name="site">Site to register.</param>
    public void AddIisSite(IisSite site)
    {
        iisSites.RemoveAll(s => s.Id == site.Id);
        iisSites.Add(site);
    }

    /// <summary>
    /// Adds a log file configuration to the server.
    /// </summary>
    /// <param name="logFile">Log file metadata.</param>
    public void AddLogFile(LogFile logFile)
    {
        logFiles.RemoveAll(l => l.Id == logFile.Id);
        logFiles.Add(logFile);
    }

    /// <summary>
    /// Records a metric point for the server.
    /// </summary>
    /// <param name="point">Metric point to append.</param>
    public void AddMetricPoint(MetricPoint point)
    {
        metricPoints.Add(point);
    }
}
