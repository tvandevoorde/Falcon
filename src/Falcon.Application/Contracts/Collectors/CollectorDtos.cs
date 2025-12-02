namespace Falcon.Application.Contracts.Collectors;

/// <summary>
/// Represents a monitoring collector resource.
/// </summary>
public sealed class CollectorDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public IDictionary<string, object>? Config { get; init; }

    public DateTimeOffset? LastSeen { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>
/// Represents a collector registration payload.
/// </summary>
public sealed class RegisterCollectorRequestDto
{
    public string Name { get; init; } = string.Empty;

    public string Type { get; init; } = "agent";

    public IDictionary<string, object>? Config { get; init; }
}

/// <summary>
/// Represents a collector heartbeat payload.
/// </summary>
public sealed class CollectorHeartbeatRequestDto
{
    public IDictionary<string, object>? Health { get; init; }

    public DateTimeOffset? LastSeen { get; init; }
}

/// <summary>
/// Represents a batch of service events pushed by a collector.
/// </summary>
public sealed class CollectorServiceEventBatchDto
{
    public IReadOnlyCollection<CollectorServiceEventDto> Events { get; init; } = Array.Empty<CollectorServiceEventDto>();
}

/// <summary>
/// Represents a service event delivered by a collector.
/// </summary>
public sealed class CollectorServiceEventDto
{
    public Guid ServiceId { get; init; }

    public string State { get; init; } = string.Empty;

    public string? Message { get; init; }

    public DateTimeOffset EventTime { get; init; }
}

/// <summary>
/// Represents a batch of log entries pushed by a collector.
/// </summary>
public sealed class CollectorLogBatchDto
{
    public IReadOnlyCollection<CollectorLogEntryDto> Entries { get; init; } = Array.Empty<CollectorLogEntryDto>();
}

/// <summary>
/// Represents a log entry pushed by a collector.
/// </summary>
public sealed class CollectorLogEntryDto
{
    public Guid LogFileId { get; init; }

    public Guid ServerId { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public string Severity { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public IDictionary<string, object>? JsonPayload { get; init; }
}