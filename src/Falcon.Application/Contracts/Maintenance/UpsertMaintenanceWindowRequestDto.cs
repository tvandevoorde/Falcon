namespace Falcon.Application.Contracts.Maintenance;

/// <summary>
/// Represents a request to create or update a maintenance window.
/// </summary>
public sealed class UpsertMaintenanceWindowRequestDto
{
    public string Name { get; init; } = string.Empty;

    public DateTimeOffset StartTime { get; init; }

    public DateTimeOffset EndTime { get; init; }

    public bool Muted { get; init; } = true;

    public IReadOnlyCollection<Guid>? Servers { get; init; }
}