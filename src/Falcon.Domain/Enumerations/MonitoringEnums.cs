namespace Falcon.Domain.Enumerations;

/// <summary>
/// Enumerates known server operational states for heartbeat reporting.
/// </summary>
public enum ServerStatus
{
    Healthy,
    Warning,
    Down,
    Unknown
}

/// <summary>
/// Enumerates canonical environment labels used for deployment segmentation.
/// </summary>
public enum EnvironmentType
{
    Dev,
    Test,
    Prod
}

/// <summary>
/// Enumerates the desired lifecycle state for a monitored Windows service.
/// </summary>
public enum ServiceDesiredState
{
    Running,
    Stopped
}

/// <summary>
/// Enumerates runtime states reported by the Windows service control manager.
/// </summary>
public enum ServiceState
{
    Running,
    Stopped,
    Paused,
    Unknown
}

/// <summary>
/// Enumerates possible execution outcomes for a scheduled task run.
/// </summary>
public enum TaskRunResult
{
    Success,
    Failure,
    Timeout,
    Cancelled,
    Unknown
}

/// <summary>
/// Represents alert severity categories leveraged for notification routing.
/// </summary>
public enum AlertSeverity
{
    Info,
    Warning,
    Critical
}

/// <summary>
/// Represents alert lifecycle states leveraged for incident workflows.
/// </summary>
public enum AlertStatus
{
    Open,
    Acknowledged,
    Closed
}

/// <summary>
/// Enumerates supported notification channels for outbound alert delivery.
/// </summary>
public enum NotificationChannel
{
    Email,
    Teams,
    Slack,
    Webhook
}

/// <summary>
/// Enumerates delivery states for alert notification attempts.
/// </summary>
public enum NotificationStatus
{
    Pending,
    Sent,
    Failed
}

/// <summary>
/// Represents collector agent hosting approaches used for data ingestion.
/// </summary>
public enum CollectorType
{
    Agent,
    WinRm,
    PowerShell,
    Hybrid
}