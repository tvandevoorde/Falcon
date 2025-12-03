namespace Falcon.Application.Contracts.Admin;

/// <summary>
/// Represents a request to upsert a notification channel configuration.
/// </summary>
public sealed class UpsertNotificationChannelRequestDto
{
    public Guid? Id { get; init; }

    public string Channel { get; init; } = string.Empty;

    public IDictionary<string, object>? Settings { get; init; }
}