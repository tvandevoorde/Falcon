using Falcon.Domain.Enumerations;

namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a single execution of a scheduled task.
/// </summary>
public sealed class TaskRun(
    Guid id,
    Guid scheduledTaskId,
    DateTimeOffset? startTime,
    DateTimeOffset? endTime,
    TaskRunResult result,
    int? exitCode,
    string? output)
{
    public Guid Id { get; } = id;

    public Guid ScheduledTaskId { get; } = scheduledTaskId;

    public DateTimeOffset? StartTime { get; } = startTime;

    public DateTimeOffset? EndTime { get; } = endTime;

    public TaskRunResult Result { get; } = result;

    public int? ExitCode { get; } = exitCode;

    public string? Output { get; } = output;
}
