namespace Falcon.Application.Contracts.Servers;

/// <summary>
/// Represents the latest metrics snapshot aggregated for a server.
/// </summary>
public sealed class ServerMetricSnapshotDto
{
    public double? CpuPercent { get; init; }

    public double? MemoryPercent { get; init; }

    public double? DiskFreePercent { get; init; }
}