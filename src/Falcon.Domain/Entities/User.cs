namespace Falcon.Domain.Entities;

/// <summary>
/// Represents an authenticated user inside the platform.
/// </summary>
public sealed class User(Guid id, string username, string email)
{
    private readonly List<RoleAssignment> roleAssignments = [];

    public Guid Id { get; } = id;

    public string Username { get; private set; } = username;

    public string? DisplayName { get; private set; }

    public string Email { get; private set; } = email;

    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    public IReadOnlyCollection<RoleAssignment> RoleAssignments => roleAssignments.AsReadOnly();

    /// <summary>
    /// Updates the profile metadata for the user.
    /// </summary>
    /// <param name="displayName">Friendly display name.</param>
    /// <param name="email">Updated email address.</param>
    public void UpdateProfile(string? displayName, string email)
    {
        DisplayName = displayName;
        Email = email;
    }

    /// <summary>
    /// Assigns a role to the user with optional scope data.
    /// </summary>
    /// <param name="roleAssignment">Role assignment payload.</param>
    public void AddRole(RoleAssignment roleAssignment)
    {
        roleAssignments.RemoveAll(r => r.RoleId == roleAssignment.RoleId);
        roleAssignments.Add(roleAssignment);
    }
}
