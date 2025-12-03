namespace Falcon.Application.Contracts.Services;

/// <summary>
/// Represents a summary of service monitoring per server.
/// </summary>
public sealed class ServiceSummaryDto
{
    public int Total { get; init; }

    public int Running { get; init; }

    public int Stopped { get; init; }

    public int CriticalAlerts { get; init; }
}
