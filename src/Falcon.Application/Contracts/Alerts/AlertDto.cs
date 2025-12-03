namespace Falcon.Application.Contracts.Alerts;

/// <summary>
/// Represents an alert entry returned by the API.
/// </summary>
public sealed record class AlertDto
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

    public IReadOnlyCollection<Guid> RelatedLogs { get; init; } = [];

    public IReadOnlyCollection<NotificationDto> Notifications { get; init; } = [];
}
