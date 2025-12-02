using Falcon.Application.Contracts.Alerts;
using Falcon.Application.Contracts.Services;
using Falcon.Application.Contracts.Tasks;
using Falcon.Application.Contracts.Iis;

namespace Falcon.Application.Contracts.Servers;

/// <summary>
/// Represents a detailed view of a monitored server.
/// </summary>
public sealed class ServerDetailDto : ServerSummaryDto
{
    public ServerMetricSnapshotDto? Metrics { get; init; }

    public IReadOnlyCollection<AlertSummaryDto> RecentAlerts { get; init; } = Array.Empty<AlertSummaryDto>();

    public ServiceSummaryDto? ServiceSummary { get; init; }

    public TaskSummaryDto? TasksSummary { get; init; }

    public IisSummaryDto? IisSummary { get; init; }
}

/// <summary>
/// Represents the latest metrics snapshot aggregated for a server.
/// </summary>
public sealed class ServerMetricSnapshotDto
{
    public double? CpuPercent { get; init; }

    public double? MemoryPercent { get; init; }

    public double? DiskFreePercent { get; init; }
}