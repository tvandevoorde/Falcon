namespace Falcon.Domain.Enumerations;

/// <summary>
/// Represents alert lifecycle states leveraged for incident workflows.
/// </summary>
public enum AlertStatus
{
    Open,
    Acknowledged,
    Closed
}
