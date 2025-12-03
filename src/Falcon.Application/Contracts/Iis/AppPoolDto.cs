namespace Falcon.Application.Contracts.Iis;

/// <summary>
/// Represents an IIS application pool entry.
/// </summary>
public sealed class AppPoolDto
{
    public Guid Id { get; init; }

    public Guid ServerId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string State { get; init; } = string.Empty;

    public DateTimeOffset? LastRecycle { get; init; }
}
