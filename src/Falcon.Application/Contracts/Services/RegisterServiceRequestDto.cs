namespace Falcon.Application.Contracts.Services;

/// <summary>
/// Represents the payload to register a service for monitoring.
/// </summary>
public sealed class RegisterServiceRequestDto
{
    public string ServiceName { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public string DesiredState { get; init; } = "running";

    public bool Critical { get; init; } = true;
}
