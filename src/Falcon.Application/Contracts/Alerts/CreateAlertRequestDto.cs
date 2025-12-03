namespace Falcon.Application.Contracts.Alerts;

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
