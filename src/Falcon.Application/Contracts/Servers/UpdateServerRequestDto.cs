namespace Falcon.Application.Contracts.Servers;

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

    public bool? IsManaged { get; init; }
}
