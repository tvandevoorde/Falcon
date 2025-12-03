namespace Falcon.Domain.Entities;

/// <summary>
/// Represents the association between a user and a role with optional scope.
/// </summary>
public sealed class RoleAssignment(Guid id, Guid userId, Guid roleId, IDictionary<string, object>? scope)
{
    public Guid Id { get; } = id;

    public Guid UserId { get; } = userId;

    public Guid RoleId { get; } = roleId;

    public IDictionary<string, object>? Scope { get; } = scope;
}
