namespace Falcon.Application.Contracts.Admin;

/// <summary>
/// Represents a request to create a role definition.
/// </summary>
public sealed class CreateRoleRequestDto
{
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}
