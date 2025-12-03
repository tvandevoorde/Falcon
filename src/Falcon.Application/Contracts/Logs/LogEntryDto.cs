namespace Falcon.Application.Contracts.Logs;

/// <summary>
/// Represents a parsed log entry returned in search results.
/// </summary>
public sealed class LogEntryDto
{
    public Guid Id { get; init; }

    public Guid LogFileId { get; init; }

    public Guid ServerId { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public string Severity { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public IDictionary<string, object>? JsonPayload { get; init; }
}
