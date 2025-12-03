namespace Falcon.Application.Contracts.Alerts;

/// <summary>
/// Represents a request to close an alert.
/// </summary>
public sealed class CloseAlertRequestDto
{
    public string? Resolution { get; init; }
}