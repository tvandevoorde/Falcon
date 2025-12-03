namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a log file monitored for patterns and critical events.
/// </summary>
public sealed class LogFile(Guid id, Guid serverId, string path, string? parser)
{
    private readonly List<LogEntry> entries = [];

    public Guid Id { get; } = id;

    public Guid ServerId { get; } = serverId;

    public string Path { get; } = path;

    public string? Parser { get; } = parser;

    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    public IReadOnlyCollection<LogEntry> Entries => entries.AsReadOnly();

    /// <summary>
    /// Appends a parsed log entry to the collection.
    /// </summary>
    /// <param name="entry">Log entry to append.</param>
    public void AddEntry(LogEntry entry)
    {
        entries.Add(entry);
    }
}
