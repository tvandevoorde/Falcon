namespace Falcon.Domain.Entities;

/// <summary>
/// Represents an outbound notification channel configuration.
/// </summary>
public sealed class NotificationChannel(Guid id, string channel, IDictionary<string, object>? settings)
{
    public Guid Id { get; } = id;

    public string Channel { get; private set; } = channel;

    public IDictionary<string, object>? Settings { get; private set; } = settings;

    /// <summary>
    /// Updates channel metadata and settings.
    /// </summary>
    /// <param name="channel">Channel identifier.</param>
    /// <param name="settings">Channel settings.</param>
    public void Update(string channel, IDictionary<string, object>? settings)
    {
        Channel = channel;
        Settings = settings;
    }
}