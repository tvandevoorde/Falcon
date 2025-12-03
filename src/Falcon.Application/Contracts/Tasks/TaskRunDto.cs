namespace Falcon.Application.Contracts.Tasks;

/// <summary>
/// Represents a single run result for a scheduled task.
/// </summary>
public sealed class TaskRunDto
{
    public Guid Id { get; init; }

    public Guid ScheduledTaskId { get; init; }

    public DateTimeOffset? StartTime { get; init; }

    public DateTimeOffset? EndTime { get; init; }

    public string Result { get; init; } = string.Empty;

    public int? ExitCode { get; init; }

    public string? Output { get; init; }
}
