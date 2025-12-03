namespace Falcon.Application.Contracts.Common;

/// <summary>
/// Represents a metric time-series response aligned with the OpenAPI contract.
/// </summary>
public sealed class MetricSeriesResponseDto
{
    public string Metric { get; init; } = string.Empty;

    public IReadOnlyCollection<MetricSeriesPointDto> Points { get; init; } = [];
}
