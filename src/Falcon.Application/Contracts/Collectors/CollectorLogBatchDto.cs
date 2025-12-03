namespace Falcon.Application.Contracts.Collectors;

/// <summary>
/// Represents a batch of log entries pushed by a collector.
/// </summary>
public sealed class CollectorLogBatchDto
{
    public IReadOnlyCollection<CollectorLogEntryDto> Entries { get; init; } = [];
}
