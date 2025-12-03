namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a sampled metric captured for a server.
/// </summary>
public sealed class MetricPoint(long id, Guid serverId, string metricName, double metricValue, DateTimeOffset measuredAt)
{
    public long Id { get; } = id;

    public Guid ServerId { get; } = serverId;

    public string MetricName { get; } = metricName;

    public double MetricValue { get; } = metricValue;

    public DateTimeOffset MeasuredAt { get; } = measuredAt;
}