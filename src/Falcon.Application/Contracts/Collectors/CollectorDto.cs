namespace Falcon.Application.Contracts.Collectors;

/// <summary>
/// Represents a monitoring collector resource.
/// </summary>
public sealed class CollectorDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public IDictionary<string, object>? Config { get; init; }

    public DateTimeOffset? LastSeen { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
