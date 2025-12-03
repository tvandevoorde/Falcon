namespace Falcon.Application.Contracts.Servers;

/// <summary>
/// Represents a request to restart a service on a server by name.
/// </summary>
public sealed class RestartServerServiceRequestDto
{
    public string ServiceName { get; init; } = string.Empty;

    public string? Reason { get; init; }
}