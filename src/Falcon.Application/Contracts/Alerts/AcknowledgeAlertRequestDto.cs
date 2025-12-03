namespace Falcon.Application.Contracts.Alerts;

/// <summary>
/// Represents a request to acknowledge an alert.
/// </summary>
public sealed class AcknowledgeAlertRequestDto
{
    public string? Comment { get; init; }
}
