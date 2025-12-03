namespace Falcon.Domain.Enumerations;

/// <summary>
/// Enumerates runtime states reported by the Windows service control manager.
/// </summary>
public enum ServiceState
{
    Running,
    Stopped,
    Paused,
    Unknown
}
