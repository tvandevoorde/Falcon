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

/// <summary>
/// Represents aggregate task monitoring statistics for a server.
/// </summary>
public sealed class TaskSummaryDto
{
    public int Total { get; init; }

    public int Enabled { get; init; }

    public int Disabled { get; init; }

    public int FailedLastRun { get; init; }
}

/// <summary>
/// Represents a manual task trigger request.
/// </summary>
public sealed class TriggerTaskRequestDto
{
    public string? Reason { get; init; }
}