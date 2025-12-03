namespace Falcon.Application.Contracts.Common;

/// <summary>
/// Represents a single time-series datapoint in a metric response.
/// </summary>
public sealed class MetricSeriesPointDto
{
    public DateTimeOffset Timestamp { get; init; }

    public double Value { get; init; }
}