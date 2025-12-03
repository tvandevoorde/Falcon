namespace Falcon.Application.Contracts.Collectors;

/// <summary>
/// Represents a collector registration payload.
/// </summary>
public sealed class RegisterCollectorRequestDto
{
    public string Name { get; init; } = string.Empty;

    public string Type { get; init; } = "agent";

    public IDictionary<string, object>? Config { get; init; }
}
