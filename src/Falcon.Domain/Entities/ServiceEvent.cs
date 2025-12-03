namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a service state transition or informational event.
/// </summary>
public sealed class ServiceEvent(Guid id, Guid serviceId, string state, string? message, DateTimeOffset eventTime)
{
    public Guid Id { get; } = id;

    public Guid ServiceId { get; } = serviceId;

    public string State { get; } = state;

    public string? Message { get; } = message;

    public DateTimeOffset EventTime { get; } = eventTime;
}
