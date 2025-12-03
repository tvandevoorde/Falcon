namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a Windows scheduled task tracked by the platform.
/// </summary>
public sealed class ScheduledTask(
    Guid id,
    Guid serverId,
    string taskName,
    bool isEnabled)
{
    private readonly List<TaskRun> taskRuns = [];

    public Guid Id { get; } = id;

    public Guid ServerId { get; } = serverId;

    public string TaskName { get; } = taskName;

    public string? ScheduleDescription { get; private set; }

    public bool IsEnabled { get; private set; } = isEnabled;

    public DateTimeOffset? LastRunTime { get; private set; }

    public string? LastRunResult { get; private set; }

    public DateTimeOffset? NextRunTime { get; private set; }

    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    public IReadOnlyCollection<TaskRun> Runs => taskRuns.AsReadOnly();

    /// <summary>
    /// Updates scheduling details for the task.
    /// </summary>
    /// <param name="scheduleDescription">Human readable schedule.</param>
    /// <param name="nextRun">Next execution time.</param>
    public void UpdateSchedule(string? scheduleDescription, DateTimeOffset? nextRun)
    {
        ScheduleDescription = scheduleDescription;
        NextRunTime = nextRun;
    }

    /// <summary>
    /// Records the outcome of the most recent run.
    /// </summary>
    /// <param name="run">Run metadata to append.</param>
    public void RecordRun(TaskRun run)
    {
        taskRuns.Add(run);
        LastRunTime = run.EndTime ?? run.StartTime;
        LastRunResult = run.Result.ToString();
    }

    /// <summary>
    /// Sets whether the task is enabled for execution.
    /// </summary>
    /// <param name="enabled">Enablement flag.</param>
    public void SetEnabled(bool enabled)
    {
        IsEnabled = enabled;
    }
}
