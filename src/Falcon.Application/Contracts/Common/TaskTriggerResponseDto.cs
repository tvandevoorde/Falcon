namespace Falcon.Application.Contracts.Common;

/// <summary>
/// Represents the response payload when a scheduled task is triggered.
/// </summary>
public sealed class TaskTriggerResponseDto
{
    public Guid RunId { get; init; }

    public DateTimeOffset StartTime { get; init; }

    public string Status { get; init; } = string.Empty;
}
