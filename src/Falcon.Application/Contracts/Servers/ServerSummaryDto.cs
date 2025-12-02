namespace Falcon.Application.Contracts.Servers;

/// <summary>
/// Represents a summary view of a monitored server.
/// </summary>
public sealed class ServerSummaryDto
{
    public Guid Id { get; init; }

    public string Hostname { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public string Environment { get; init; } = string.Empty;

    public string? IpAddress { get; init; }

    public string? Os { get; init; }

    public DateTimeOffset? LastHeartbeat { get; init; }

    public string Status { get; init; } = string.Empty;

    public double? Cpu { get; init; }

    public double? MemoryPercent { get; init; }

    public IReadOnlyCollection<string> Tags { get; init; } = Array.Empty<string>();
}