namespace Falcon.Application.Contracts.Services;

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