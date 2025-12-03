namespace Falcon.Application.Contracts.Iis;

/// <summary>
/// Represents an IIS site entry.
/// </summary>
public sealed class IisSiteDto
{
    public Guid Id { get; init; }

    public Guid ServerId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public IDictionary<string, string>? Bindings { get; init; }

    public string? PingEndpoint { get; init; }

    public int? LastHttpStatus { get; init; }

    public DateTimeOffset? LastChecked { get; init; }
}
