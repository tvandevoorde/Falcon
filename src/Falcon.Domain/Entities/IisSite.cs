namespace Falcon.Domain.Entities;

/// <summary>
/// Represents an IIS site and its binding metadata.
/// </summary>
public sealed class IisSite(Guid id, Guid serverId, string name, string status)
{
    public Guid Id { get; } = id;

    public Guid ServerId { get; } = serverId;

    public string Name { get; } = name;

    public string Status { get; private set; } = status;

    public IDictionary<string, string>? Bindings { get; private set; }

    public string? PingEndpoint { get; private set; }

    public int? LastHttpStatus { get; private set; }

    public DateTimeOffset? LastChecked { get; private set; }

    /// <summary>
    /// Updates operational metadata for the site.
    /// </summary>
    /// <param name="status">Operational status.</param>
    /// <param name="bindings">Binding metadata.</param>
    /// <param name="pingEndpoint">Health check endpoint.</param>
    /// <param name="lastHttpStatus">HTTP status recorded.</param>
    /// <param name="lastChecked">Timestamp of last check.</param>
    public void UpdateMetadata(
        string status,
        IDictionary<string, string>? bindings,
        string? pingEndpoint,
        int? lastHttpStatus,
        DateTimeOffset? lastChecked)
    {
        Status = status;
        Bindings = bindings;
        PingEndpoint = pingEndpoint;
        LastHttpStatus = lastHttpStatus;
        LastChecked = lastChecked;
    }
}
