namespace Falcon.Application.Contracts.Admin;

/// <summary>
/// Represents a configurable notification channel.
/// </summary>
public sealed class NotificationChannelDto
{
    public Guid Id { get; init; }

    public string Channel { get; init; } = string.Empty;

    public IDictionary<string, object>? Settings { get; init; }
}
