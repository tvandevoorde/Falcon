namespace Falcon.Application.Contracts.Alerts;

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
