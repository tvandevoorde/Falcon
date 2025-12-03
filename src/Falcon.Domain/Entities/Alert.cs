using Falcon.Domain.Enumerations;

namespace Falcon.Domain.Entities;

/// <summary>
/// Represents an alert raised by the monitoring platform.
/// </summary>
public sealed class Alert(
    Guid id,
    Guid? serverId,
    string sourceType,
    Guid? sourceId,
    string alertType,
    AlertSeverity severity,
    string status,
    string message,
    DateTimeOffset createdAt)
{
    private readonly List<Notification> notifications = [];

    public Guid Id { get; } = id;

    public Guid? ServerId { get; } = serverId;

    public string SourceType { get; } = sourceType;

    public Guid? SourceId { get; } = sourceId;

    public string AlertType { get; } = alertType;

    public AlertSeverity Severity { get; private set; } = severity;

    public string Status { get; private set; } = status;

    public string Message { get; private set; } = message;

    public DateTimeOffset CreatedAt { get; } = createdAt;

    public DateTimeOffset? ResolvedAt { get; private set; }

    public IReadOnlyCollection<Notification> Notifications => notifications.AsReadOnly();

    public IReadOnlyCollection<Guid> RelatedLogIds => relatedLogIds.AsReadOnly();

    private readonly List<Guid> relatedLogIds = [];

    /// <summary>
    /// Associates a log entry with the alert for easier correlation.
    /// </summary>
    /// <param name="logId">Log entry identifier.</param>
    public void AddRelatedLog(Guid logId)
    {
        if (!relatedLogIds.Contains(logId))
        {
            relatedLogIds.Add(logId);
        }
    }

    /// <summary>
    /// Updates the human readable message and severity of the alert.
    /// </summary>
    /// <param name="message">New message to store.</param>
    /// <param name="severity">New severity classification.</param>
    public void UpdateMessage(string message, AlertSeverity severity)
    {
        Message = message;
        Severity = severity;
    }

    /// <summary>
    /// Transitions the alert to a new status.
    /// </summary>
    /// <param name="status">Lifecycle status (open, acknowledged, closed).</param>
    /// <param name="resolvedAt">Optional resolution timestamp.</param>
    public void UpdateStatus(string status, DateTimeOffset? resolvedAt)
    {
        Status = status;
        ResolvedAt = resolvedAt;
    }

    /// <summary>
    /// Adds a notification record to the alert history.
    /// </summary>
    /// <param name="notification">Notification instance.</param>
    public void AddNotification(Notification notification)
    {
        notifications.Add(notification);
    }
}
