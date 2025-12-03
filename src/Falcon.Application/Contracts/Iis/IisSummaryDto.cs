namespace Falcon.Application.Contracts.Iis;

/// <summary>
/// Represents IIS monitoring summary metrics per server.
/// </summary>
public sealed class IisSummaryDto
{
    public int AppPools { get; init; }

    public int Sites { get; init; }

    public int StoppedAppPools { get; init; }

    public int UnhealthySites { get; init; }
}
