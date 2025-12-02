namespace Falcon.Application.Contracts.Services;

/// <summary>
/// Represents a monitored Windows service resource.
/// </summary>
public sealed class MonitoredServiceDto
{
    public Guid Id { get; init; }

    public Guid ServerId { get; init; }

    public string ServiceName { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public string DesiredState { get; init; } = string.Empty;

    public string CurrentState { get; init; } = string.Empty;

    public bool Critical { get; init; }

    public DateTimeOffset? LastChange { get; init; }
}

/// <summary>
/// Represents a summary of service monitoring per server.
/// </summary>
public sealed class ServiceSummaryDto
{
    public int Total { get; init; }

    public int Running { get; init; }

    public int Stopped { get; init; }

    public int CriticalAlerts { get; init; }
}

/// <summary>
/// Represents the payload to register a service for monitoring.
/// </summary>
public sealed class RegisterServiceRequestDto
{
    public string ServiceName { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public string DesiredState { get; init; } = "running";

    public bool Critical { get; init; } = true;
}

/// <summary>
/// Represents a request to trigger a service restart.
/// </summary>
public sealed class RestartServiceRequestDto
{
    public string? Reason { get; init; }
}

/// <summary>
/// Represents an individual service event entry.
/// </summary>
public sealed class ServiceEventDto
{
    public Guid Id { get; init; }

    public Guid ServiceId { get; init; }

    public string State { get; init; } = string.Empty;

    public string? Message { get; init; }

    public DateTimeOffset EventTime { get; init; }
}