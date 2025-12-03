namespace Falcon.Application.Contracts.Admin;

/// <summary>
/// Represents a request to create a user within the platform.
/// </summary>
public sealed class CreateUserRequestDto
{
    public string Username { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public IReadOnlyCollection<Guid>? RoleIds { get; init; }
}
