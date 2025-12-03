namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a planned maintenance window used to mute alerts.
/// </summary>
public sealed class MaintenanceWindow(Guid id, string name, DateTimeOffset startTime, DateTimeOffset endTime, bool muted)
{
    public Guid Id { get; } = id;

    public string Name { get; private set; } = name;

    public DateTimeOffset StartTime { get; private set; } = startTime;

    public DateTimeOffset EndTime { get; private set; } = endTime;

    public bool Muted { get; private set; } = muted;

    public IReadOnlyCollection<Guid> ServerScope => serverScope.AsReadOnly();

    private readonly List<Guid> serverScope = [];

    /// <summary>
    /// Assigns the servers included in the maintenance window scope.
    /// </summary>
    /// <param name="serverIds">Collection of server identifiers.</param>
    public void SetServerScope(IEnumerable<Guid> serverIds)
    {
        serverScope.Clear();
        serverScope.AddRange(serverIds);
    }

    /// <summary>
    /// Updates the scheduling metadata for the window.
    /// </summary>
    /// <param name="name">Window name.</param>
    /// <param name="start">Start timestamp.</param>
    /// <param name="end">End timestamp.</param>
    /// <param name="muted">Whether alerts should be muted.</param>
    public void Update(string name, DateTimeOffset start, DateTimeOffset end, bool muted)
    {
        Name = name;
        StartTime = start;
        EndTime = end;
        Muted = muted;
    }
}
