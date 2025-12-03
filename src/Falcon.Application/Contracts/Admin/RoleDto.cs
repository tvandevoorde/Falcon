namespace Falcon.Application.Contracts.Admin;

/// <summary>
/// Represents a platform role entry.
/// </summary>
public sealed class RoleDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}
