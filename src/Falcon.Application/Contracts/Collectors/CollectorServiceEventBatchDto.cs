namespace Falcon.Application.Contracts.Collectors;

/// <summary>
/// Represents a batch of service events pushed by a collector.
/// </summary>
public sealed class CollectorServiceEventBatchDto
{
    public IReadOnlyCollection<CollectorServiceEventDto> Events { get; init; } = [];
}
