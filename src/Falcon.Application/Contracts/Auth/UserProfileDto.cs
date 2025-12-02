namespace Falcon.Application.Contracts.Auth;

/// <summary>
/// Represents the authenticated user profile result.
/// </summary>
public sealed class UserProfileDto
{
    public Guid Id { get; init; }

    public string Username { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public string Email { get; init; } = string.Empty;

    public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();

    public DateTimeOffset CreatedAt { get; init; }
}