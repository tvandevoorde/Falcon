namespace Falcon.Domain.Enumerations;

/// <summary>
/// Enumerates known server operational states for heartbeat reporting.
/// </summary>
public enum ServerStatus
{
    Healthy,
    Warning,
    Down,
    Unknown
}
