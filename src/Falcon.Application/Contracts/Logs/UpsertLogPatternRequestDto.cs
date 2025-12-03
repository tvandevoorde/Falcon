namespace Falcon.Application.Contracts.Logs;

/// <summary>
/// Represents the payload to create or update a log pattern.
/// </summary>
public sealed class UpsertLogPatternRequestDto
{
    public string Name { get; init; } = string.Empty;

    public string Pattern { get; init; } = string.Empty;

    public string SeverityDefault { get; init; } = "warning";

    public bool Enabled { get; init; } = true;
}