namespace Falcon.Domain.Enumerations;

/// <summary>
/// Represents collector agent hosting approaches used for data ingestion.
/// </summary>
public enum MonitoringEnums
{
    Agent,
    WinRm,
    PowerShell,
    Hybrid
}