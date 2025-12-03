namespace Falcon.Application.Contracts.Tasks;

/// <summary>
/// Represents a scheduled task configuration associated with a server.
/// </summary>
public sealed class ScheduledTaskDto
{
    public Guid Id { get; init; }

    public Guid ServerId { get; init; }

    public string TaskName { get; init; } = string.Empty;

    public string? ScheduleDesc { get; init; }

    public bool IsEnabled { get; init; }

    public DateTimeOffset? LastRunTime { get; init; }

    public string? LastRunResult { get; init; }

    public DateTimeOffset? NextRunTime { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
