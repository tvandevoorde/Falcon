namespace Falcon.Application.Contracts.Servers;

/// <summary>
/// Represents a single metric point for a server metric query.
/// </summary>
public sealed class MetricPointDto
{
    public long Id { get; init; }

    public Guid ServerId { get; init; }

    public string MetricName { get; init; } = string.Empty;

    public double MetricValue { get; init; }

    public DateTimeOffset MeasuredAt { get; init; }
}