namespace Falcon.Application.Contracts.Alerts;

/// <summary>
/// Represents a notification attempt associated with an alert.
/// </summary>
public sealed record class NotificationDto
{
    public Guid Id { get; init; }

    public Guid AlertId { get; init; }

    public string Channel { get; init; } = string.Empty;

    public string Recipient { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public int AttemptCount { get; init; }

    public DateTimeOffset? LastAttempt { get; init; }

    public IDictionary<string, object>? Payload { get; init; }
}
