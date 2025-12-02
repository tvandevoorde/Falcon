using Falcon.Domain.Enumerations;

namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a monitored Windows server along with its dependent resources.
/// </summary>
public sealed class Server
{
    private readonly List<MonitoredService> services = new();
    private readonly List<ScheduledTask> scheduledTasks = new();
    private readonly List<AppPool> appPools = new();
    private readonly List<IisSite> iisSites = new();
    private readonly List<LogFile> logFiles = new();
    private readonly List<MetricPoint> metricPoints = new();

    public Server(
        Guid id,
        string hostname,
        EnvironmentType environment,
        ServerStatus status)
    {
        Id = id;
        Hostname = hostname;
        Environment = environment;
        Status = status;
    }

    public Guid Id { get; }

    public string Hostname { get; private set; }

    public string? DisplayName { get; private set; }

    public EnvironmentType Environment { get; private set; }

    public string? IpAddress { get; private set; }

    public string? OperatingSystem { get; private set; }

    public DateTimeOffset? LastHeartbeat { get; private set; }

    public ServerStatus Status { get; private set; }

    public double? CpuPercent { get; private set; }

    public double? MemoryPercent { get; private set; }

    public IReadOnlyCollection<string> Tags => tags.AsReadOnly();

    public IReadOnlyCollection<MonitoredService> Services => services.AsReadOnly();

    public IReadOnlyCollection<ScheduledTask> ScheduledTasks => scheduledTasks.AsReadOnly();

    public IReadOnlyCollection<AppPool> AppPools => appPools.AsReadOnly();

    public IReadOnlyCollection<IisSite> IisSites => iisSites.AsReadOnly();

    public IReadOnlyCollection<LogFile> LogFiles => logFiles.AsReadOnly();

    public IReadOnlyCollection<MetricPoint> MetricPoints => metricPoints.AsReadOnly();

    private readonly List<string> tags = new();

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

/// <summary>
/// Represents a Windows service monitored by the platform.
/// </summary>
public sealed class MonitoredService
{
    private readonly List<ServiceEvent> events = new();

    public MonitoredService(
        Guid id,
        Guid serverId,
        string serviceName,
        ServiceDesiredState desiredState,
        ServiceState currentState,
        bool critical)
    {
        Id = id;
        ServerId = serverId;
        ServiceName = serviceName;
        DesiredState = desiredState;
        CurrentState = currentState;
        Critical = critical;
    }

    public Guid Id { get; }

    public Guid ServerId { get; }

    public string ServiceName { get; }

    public string? DisplayName { get; init; }

    public ServiceDesiredState DesiredState { get; private set; }

    public ServiceState CurrentState { get; private set; }

    public bool Critical { get; private set; }

    public DateTimeOffset? LastChange { get; private set; }

    public IReadOnlyCollection<ServiceEvent> Events => events.AsReadOnly();

    /// <summary>
    /// Updates the runtime state for the service.
    /// </summary>
    /// <param name="state">New runtime state.</param>
    /// <param name="changedAt">Timestamp of the change.</param>
    public void UpdateState(ServiceState state, DateTimeOffset? changedAt)
    {
        CurrentState = state;
        LastChange = changedAt;
    }

    /// <summary>
    /// Adjusts the desired state for orchestration purposes.
    /// </summary>
    /// <param name="desiredState">Desired target state.</param>
    public void UpdateDesiredState(ServiceDesiredState desiredState)
    {
        DesiredState = desiredState;
    }

    /// <summary>
    /// Updates the criticality flag for alerting purposes.
    /// </summary>
    /// <param name="critical">Whether the service is critical.</param>
    public void UpdateCriticality(bool critical)
    {
        Critical = critical;
    }

    /// <summary>
    /// Records a service event in chronological order.
    /// </summary>
    /// <param name="serviceEvent">Event to append.</param>
    public void AddEvent(ServiceEvent serviceEvent)
    {
        events.Add(serviceEvent);
    }
}

/// <summary>
/// Represents a service state transition or informational event.
/// </summary>
public sealed class ServiceEvent
{
    public ServiceEvent(Guid id, Guid serviceId, string state, string? message, DateTimeOffset eventTime)
    {
        Id = id;
        ServiceId = serviceId;
        State = state;
        Message = message;
        EventTime = eventTime;
    }

    public Guid Id { get; }

    public Guid ServiceId { get; }

    public string State { get; }

    public string? Message { get; }

    public DateTimeOffset EventTime { get; }
}

/// <summary>
/// Represents a Windows scheduled task tracked by the platform.
/// </summary>
public sealed class ScheduledTask
{
    private readonly List<TaskRun> taskRuns = new();

    public ScheduledTask(
        Guid id,
        Guid serverId,
        string taskName,
        bool isEnabled)
    {
        Id = id;
        ServerId = serverId;
        TaskName = taskName;
        IsEnabled = isEnabled;
    }

    public Guid Id { get; }

    public Guid ServerId { get; }

    public string TaskName { get; }

    public string? ScheduleDescription { get; private set; }

    public bool IsEnabled { get; private set; }

    public DateTimeOffset? LastRunTime { get; private set; }

    public string? LastRunResult { get; private set; }

    public DateTimeOffset? NextRunTime { get; private set; }

    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    public IReadOnlyCollection<TaskRun> Runs => taskRuns.AsReadOnly();

    /// <summary>
    /// Updates scheduling details for the task.
    /// </summary>
    /// <param name="scheduleDescription">Human readable schedule.</param>
    /// <param name="nextRun">Next execution time.</param>
    public void UpdateSchedule(string? scheduleDescription, DateTimeOffset? nextRun)
    {
        ScheduleDescription = scheduleDescription;
        NextRunTime = nextRun;
    }

    /// <summary>
    /// Records the outcome of the most recent run.
    /// </summary>
    /// <param name="run">Run metadata to append.</param>
    public void RecordRun(TaskRun run)
    {
        taskRuns.Add(run);
        LastRunTime = run.EndTime ?? run.StartTime;
        LastRunResult = run.Result.ToString();
    }

    /// <summary>
    /// Sets whether the task is enabled for execution.
    /// </summary>
    /// <param name="enabled">Enablement flag.</param>
    public void SetEnabled(bool enabled)
    {
        IsEnabled = enabled;
    }
}

/// <summary>
/// Represents a single execution of a scheduled task.
/// </summary>
public sealed class TaskRun
{
    public TaskRun(
        Guid id,
        Guid scheduledTaskId,
        DateTimeOffset? startTime,
        DateTimeOffset? endTime,
        TaskRunResult result,
        int? exitCode,
        string? output)
    {
        Id = id;
        ScheduledTaskId = scheduledTaskId;
        StartTime = startTime;
        EndTime = endTime;
        Result = result;
        ExitCode = exitCode;
        Output = output;
    }

    public Guid Id { get; }

    public Guid ScheduledTaskId { get; }

    public DateTimeOffset? StartTime { get; }

    public DateTimeOffset? EndTime { get; }

    public TaskRunResult Result { get; }

    public int? ExitCode { get; }

    public string? Output { get; }
}

/// <summary>
/// Represents an IIS application pool monitored for stability.
/// </summary>
public sealed class AppPool
{
    public AppPool(Guid id, Guid serverId, string name, string state)
    {
        Id = id;
        ServerId = serverId;
        Name = name;
        State = state;
    }

    public Guid Id { get; }

    public Guid ServerId { get; }

    public string Name { get; }

    public string State { get; private set; }

    public DateTimeOffset? LastRecycle { get; private set; }

    /// <summary>
    /// Sets the runtime state of the application pool.
    /// </summary>
    /// <param name="state">New state value.</param>
    /// <param name="lastRecycle">Timestamp of last recycle.</param>
    public void UpdateState(string state, DateTimeOffset? lastRecycle)
    {
        State = state;
        LastRecycle = lastRecycle;
    }
}

/// <summary>
/// Represents an IIS site and its binding metadata.
/// </summary>
public sealed class IisSite
{
    public IisSite(Guid id, Guid serverId, string name, string status)
    {
        Id = id;
        ServerId = serverId;
        Name = name;
        Status = status;
    }

    public Guid Id { get; }

    public Guid ServerId { get; }

    public string Name { get; }

    public string Status { get; private set; }

    public IDictionary<string, string>? Bindings { get; private set; }

    public string? PingEndpoint { get; private set; }

    public int? LastHttpStatus { get; private set; }

    public DateTimeOffset? LastChecked { get; private set; }

    /// <summary>
    /// Updates operational metadata for the site.
    /// </summary>
    /// <param name="status">Operational status.</param>
    /// <param name="bindings">Binding metadata.</param>
    /// <param name="pingEndpoint">Health check endpoint.</param>
    /// <param name="lastHttpStatus">HTTP status recorded.</param>
    /// <param name="lastChecked">Timestamp of last check.</param>
    public void UpdateMetadata(
        string status,
        IDictionary<string, string>? bindings,
        string? pingEndpoint,
        int? lastHttpStatus,
        DateTimeOffset? lastChecked)
    {
        Status = status;
        Bindings = bindings;
        PingEndpoint = pingEndpoint;
        LastHttpStatus = lastHttpStatus;
        LastChecked = lastChecked;
    }
}

/// <summary>
/// Represents a log file monitored for patterns and critical events.
/// </summary>
public sealed class LogFile
{
    private readonly List<LogEntry> entries = new();

    public LogFile(Guid id, Guid serverId, string path, string? parser)
    {
        Id = id;
        ServerId = serverId;
        Path = path;
        Parser = parser;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }

    public Guid ServerId { get; }

    public string Path { get; }

    public string? Parser { get; }

    public DateTimeOffset CreatedAt { get; }

    public IReadOnlyCollection<LogEntry> Entries => entries.AsReadOnly();

    /// <summary>
    /// Appends a parsed log entry to the collection.
    /// </summary>
    /// <param name="entry">Log entry to append.</param>
    public void AddEntry(LogEntry entry)
    {
        entries.Add(entry);
    }
}

/// <summary>
/// Represents a single parsed line from a monitored log file.
/// </summary>
public sealed class LogEntry
{
    public LogEntry(
        Guid id,
        Guid logFileId,
        Guid serverId,
        DateTimeOffset timestamp,
        string severity,
        string message,
        IDictionary<string, object>? jsonPayload)
    {
        Id = id;
        LogFileId = logFileId;
        ServerId = serverId;
        Timestamp = timestamp;
        Severity = severity;
        Message = message;
        JsonPayload = jsonPayload;
    }

    public Guid Id { get; }

    public Guid LogFileId { get; }

    public Guid ServerId { get; }

    public DateTimeOffset Timestamp { get; }

    public string Severity { get; }

    public string Message { get; }

    public IDictionary<string, object>? JsonPayload { get; }
}

/// <summary>
/// Represents a sampled metric captured for a server.
/// </summary>
public sealed class MetricPoint
{
    public MetricPoint(long id, Guid serverId, string metricName, double metricValue, DateTimeOffset measuredAt)
    {
        Id = id;
        ServerId = serverId;
        MetricName = metricName;
        MetricValue = metricValue;
        MeasuredAt = measuredAt;
    }

    public long Id { get; }

    public Guid ServerId { get; }

    public string MetricName { get; }

    public double MetricValue { get; }

    public DateTimeOffset MeasuredAt { get; }
}