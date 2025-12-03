namespace Falcon.Application.Contracts.Servers;

/// <summary>
/// Represents the payload required to register a server with the platform.
/// </summary>
public sealed class RegisterServerRequestDto
{
    public Guid? CollectorId { get; init; }

    public string Hostname { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public string Environment { get; init; } = "dev";

    public string? IpAddress { get; init; }

    public string? Os { get; init; }

    public IReadOnlyCollection<string>? Tags { get; init; }
}
