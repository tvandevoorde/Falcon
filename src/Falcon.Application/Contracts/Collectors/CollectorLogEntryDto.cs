namespace Falcon.Application.Contracts.Collectors;

/// <summary>
/// Represents a log entry pushed by a collector.
/// </summary>
public sealed class CollectorLogEntryDto
{
    public Guid LogFileId { get; init; }

    public Guid ServerId { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public string Severity { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public IDictionary<string, object>? JsonPayload { get; init; }
}