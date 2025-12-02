namespace Falcon.Application.Contracts.Admin;

/// <summary>
/// Represents an application user entry.
/// </summary>
public sealed class UserDto
{
    public Guid Id { get; init; }

    public string Username { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public string Email { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();
}

/// <summary>
/// Represents a platform role entry.
/// </summary>
public sealed class RoleDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}

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

/// <summary>
/// Represents a request to create a role definition.
/// </summary>
public sealed class CreateRoleRequestDto
{
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}

/// <summary>
/// Represents a configurable notification channel.
/// </summary>
public sealed class NotificationChannelDto
{
    public Guid Id { get; init; }

    public string Channel { get; init; } = string.Empty;

    public IDictionary<string, object>? Settings { get; init; }
}

/// <summary>
/// Represents a request to upsert a notification channel configuration.
/// </summary>
public sealed class UpsertNotificationChannelRequestDto
{
    public Guid? Id { get; init; }

    public string Channel { get; init; } = string.Empty;

    public IDictionary<string, object>? Settings { get; init; }
}