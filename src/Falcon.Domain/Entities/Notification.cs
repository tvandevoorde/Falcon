using Falcon.Domain.Enumerations;

namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a notification delivery attempt for an alert.
/// </summary>
public sealed class Notification(
    Guid id,
    Guid alertId,
    NotificationChannel channel,
    string recipient,
    NotificationStatus status,
    int attemptCount,
    DateTimeOffset? lastAttempt,
    IDictionary<string, object>? payload)
{
    public Guid Id { get; } = id;

    public Guid AlertId { get; } = alertId;

    public NotificationChannel Channel { get; } = channel;

    public string Recipient { get; } = recipient;

    public NotificationStatus Status { get; private set; } = status;

    public int AttemptCount { get; private set; } = attemptCount;

    public DateTimeOffset? LastAttempt { get; private set; } = lastAttempt;

    public IDictionary<string, object>? Payload { get; } = payload;

    /// <summary>
    /// Updates the delivery status of the notification.
    /// </summary>
    /// <param name="status">New status value.</param>
    /// <param name="attemptedAt">Timestamp of the attempt.</param>
    public void UpdateStatus(NotificationStatus status, DateTimeOffset? attemptedAt)
    {
        Status = status;
        LastAttempt = attemptedAt;
        AttemptCount++;
    }
}
