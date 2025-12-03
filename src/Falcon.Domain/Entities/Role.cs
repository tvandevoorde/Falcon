namespace Falcon.Domain.Entities;

/// <summary>
/// Represents an application role used for RBAC.
/// </summary>
public sealed class Role(Guid id, string name, string? description)
{
    public Guid Id { get; } = id;

    public string Name { get; private set; } = name;

    public string? Description { get; private set; } = description;

    /// <summary>
    /// Updates descriptive metadata about the role.
    /// </summary>
    /// <param name="name">Role name.</param>
    /// <param name="description">Role description.</param>
    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}
