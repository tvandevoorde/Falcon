namespace Falcon.Domain.Enumerations;

/// <summary>
/// Enumerates possible execution outcomes for a scheduled task run.
/// </summary>
public enum TaskRunResult
{
    Success,
    Failure,
    Timeout,
    Cancelled,
    Unknown
}
