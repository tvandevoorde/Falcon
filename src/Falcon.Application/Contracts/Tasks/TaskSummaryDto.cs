namespace Falcon.Application.Contracts.Tasks;

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
