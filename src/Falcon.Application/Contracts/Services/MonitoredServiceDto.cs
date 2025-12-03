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
