namespace Falcon.Domain.Enumerations;

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
