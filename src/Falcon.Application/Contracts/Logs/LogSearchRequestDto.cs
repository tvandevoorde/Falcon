namespace Falcon.Application.Contracts.Logs;

/// <summary>
/// Represents the request payload for the log search endpoint.
/// </summary>
public sealed class LogSearchRequestDto
{
    public IReadOnlyCollection<Guid>? ServerIds { get; init; }

    public IReadOnlyCollection<Guid>? LogFileIds { get; init; }

    public IReadOnlyCollection<string>? Severities { get; init; }

    public string? Query { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;
}
