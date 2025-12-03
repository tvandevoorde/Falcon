namespace Falcon.Application.Contracts.Maintenance;

/// <summary>
/// Represents a maintenance window definition.
/// </summary>
public sealed class MaintenanceWindowDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateTimeOffset StartTime { get; init; }

    public DateTimeOffset EndTime { get; init; }

    public bool Muted { get; init; }

    public IReadOnlyCollection<Guid> Servers { get; init; } = [];
}
