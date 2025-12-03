namespace Falcon.Domain.Entities;

/// <summary>
/// Represents an IIS application pool monitored for stability.
/// </summary>
public sealed class AppPool(Guid id, Guid serverId, string name, string state)
{
    public Guid Id { get; } = id;

    public Guid ServerId { get; } = serverId;

    public string Name { get; } = name;

    public string State { get; private set; } = state;

    public DateTimeOffset? LastRecycle { get; private set; }

    /// <summary>
    /// Sets the runtime state of the application pool.
    /// </summary>
    /// <param name="state">New state value.</param>
    /// <param name="lastRecycle">Timestamp of last recycle.</param>
    public void UpdateState(string state, DateTimeOffset? lastRecycle)
    {
        State = state;
        LastRecycle = lastRecycle;
    }
}
