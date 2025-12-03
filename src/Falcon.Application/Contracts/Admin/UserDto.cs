namespace Falcon.Application.Contracts.Admin;

/// <summary>
/// Represents an application user entry.
/// </summary>
public sealed record class UserDto
{
    public Guid Id { get; init; }

    public string Username { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public string Email { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } = [];
}
