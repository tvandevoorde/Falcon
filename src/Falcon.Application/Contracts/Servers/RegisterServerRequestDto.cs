namespace Falcon.Application.Contracts.Servers;

/// <summary>
/// Represents the payload required to register a server with the platform.
/// </summary>
public sealed class RegisterServerRequestDto
{
    public string Hostname { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public string Environment { get; init; } = "dev";

    public string? IpAddress { get; init; }

    public string? Os { get; init; }

    public IReadOnlyCollection<string>? Tags { get; init; }
}

/// <summary>
/// Represents the payload used to update server metadata.
/// </summary>
public sealed class UpdateServerRequestDto
{
    public string? DisplayName { get; init; }

    public string Environment { get; init; } = "dev";

    public string? IpAddress { get; init; }

    public string? Os { get; init; }

    public IReadOnlyCollection<string>? Tags { get; init; }
}

/// <summary>
/// Represents a request to restart a service on a server by name.
/// </summary>
public sealed class RestartServerServiceRequestDto
{
    public string ServiceName { get; init; } = string.Empty;

    public string? Reason { get; init; }
}