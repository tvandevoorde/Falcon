namespace Falcon.Application.Contracts.Collectors;

/// <summary>
/// Represents a collector heartbeat payload.
/// </summary>
public sealed class CollectorHeartbeatRequestDto
{
    public IDictionary<string, object>? Health { get; init; }

    public DateTimeOffset? LastSeen { get; init; }
}
