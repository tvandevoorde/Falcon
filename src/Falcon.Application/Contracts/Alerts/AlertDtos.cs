namespace Falcon.Application.Contracts.Alerts;

/// <summary>
/// Represents an alert entry returned by the API.
/// </summary>
public sealed class AlertDto
{
    public Guid Id { get; init; }

    public Guid? ServerId { get; init; }

    public string SourceType { get; init; } = string.Empty;

    public Guid? SourceId { get; init; }

    public string AlertType { get; init; } = string.Empty;

    public string Severity { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? ResolvedAt { get; init; }

    public IReadOnlyCollection<Guid> RelatedLogs { get; init; } = Array.Empty<Guid>();

    public IReadOnlyCollection<NotificationDto> Notifications { get; init; } = Array.Empty<NotificationDto>();
}

/// <summary>
/// Represents an alert item for summary listings.
/// </summary>
public sealed class AlertSummaryDto
{
    public Guid Id { get; init; }

    public string Severity { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>
/// Represents a notification attempt associated with an alert.
/// </summary>
public sealed class NotificationDto
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

/// <summary>
/// Represents the request payload to create a manual alert.
/// </summary>
public sealed class CreateAlertRequestDto
{
    public Guid? ServerId { get; init; }

    public string SourceType { get; init; } = string.Empty;

    public Guid? SourceId { get; init; }

    public string AlertType { get; init; } = string.Empty;

    public string Severity { get; init; } = "warning";

    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Represents a request to acknowledge an alert.
/// </summary>
public sealed class AcknowledgeAlertRequestDto
{
    public string? Comment { get; init; }
}

/// <summary>
/// Represents a request to close an alert.
/// </summary>
public sealed class CloseAlertRequestDto
{
    public string? Resolution { get; init; }
}