namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a single parsed line from a monitored log file.
/// </summary>
public sealed class LogEntry(
    Guid id,
    Guid logFileId,
    Guid serverId,
    DateTimeOffset timestamp,
    string severity,
    string message,
    IDictionary<string, object>? jsonPayload)
{
    public Guid Id { get; } = id;

    public Guid LogFileId { get; } = logFileId;

    public Guid ServerId { get; } = serverId;

    public DateTimeOffset Timestamp { get; } = timestamp;

    public string Severity { get; } = severity;

    public string Message { get; } = message;

    public IDictionary<string, object>? JsonPayload { get; } = jsonPayload;
}
