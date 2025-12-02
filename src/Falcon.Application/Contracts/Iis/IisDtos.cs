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

/// <summary>
/// Represents IIS monitoring summary metrics per server.
/// </summary>
public sealed class IisSummaryDto
{
    public int AppPools { get; init; }

    public int Sites { get; init; }

    public int StoppedAppPools { get; init; }

    public int UnhealthySites { get; init; }
}

/// <summary>
/// Represents a request to recycle an IIS application pool.
/// </summary>
public sealed class RecycleAppPoolRequestDto
{
    public string? Reason { get; init; }
}