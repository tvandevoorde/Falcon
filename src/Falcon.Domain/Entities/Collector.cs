using Falcon.Domain.Enumerations;

namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a monitoring collector agent or integration configuration.
/// </summary>
public sealed class Collector(Guid id, string name, MonitoringEnums type)
{
    public Guid Id { get; } = id;

    public string Name { get; private set; } = name;

    public MonitoringEnums Type { get; private set; } = type;

    public IDictionary<string, object>? Config { get; private set; }

    public DateTimeOffset? LastSeen { get; private set; }

    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Applies configuration payload to the collector.
    /// </summary>
    /// <param name="config">Arbitrary configuration object tree.</param>
    public void ApplyConfiguration(IDictionary<string, object>? config)
    {
        Config = config;
    }

    /// <summary>
    /// Updates the last seen heartbeat metadata.
    /// </summary>
    /// <param name="lastSeen">Timestamp when collector was last online.</param>
    public void UpdateLastSeen(DateTimeOffset lastSeen)
    {
        LastSeen = lastSeen;
    }

    /// <summary>
    /// Updates the collector display metadata.
    /// </summary>
    /// <param name="name">Collector friendly name.</param>
    /// <param name="type">Collector type enumeration.</param>
    public void UpdateMetadata(string name, MonitoringEnums type)
    {
        Name = name;
        Type = type;
    }
}
