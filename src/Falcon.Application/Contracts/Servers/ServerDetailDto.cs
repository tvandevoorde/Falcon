using Falcon.Application.Contracts.Alerts;
using Falcon.Application.Contracts.Iis;
using Falcon.Application.Contracts.Services;
using Falcon.Application.Contracts.Tasks;

namespace Falcon.Application.Contracts.Servers;

/// <summary>
/// Represents a detailed view of a monitored server.
/// </summary>
public sealed record class ServerDetailDto : ServerSummaryDto
{
    public ServerMetricSnapshotDto? Metrics { get; init; }

    public IReadOnlyCollection<AlertSummaryDto> RecentAlerts { get; init; } = [];

    public ServiceSummaryDto? ServiceSummary { get; init; }

    public TaskSummaryDto? TasksSummary { get; init; }

    public IisSummaryDto? IisSummary { get; init; }
}
