namespace Falcon.Application.Contracts.Collectors;

/// <summary>
/// Represents a service event delivered by a collector.
/// </summary>
public sealed class CollectorServiceEventDto
{
    public Guid? ServiceId { get; init; }

    public string? ServiceName { get; init; }

    public string State { get; init; } = string.Empty;

    public string? Message { get; init; }

    public DateTimeOffset EventTime { get; init; }
}
