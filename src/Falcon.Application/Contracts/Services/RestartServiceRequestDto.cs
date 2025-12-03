namespace Falcon.Application.Contracts.Services;

/// <summary>
/// Represents a request to trigger a service restart.
/// </summary>
public sealed class RestartServiceRequestDto
{
    public string? Reason { get; init; }
}
