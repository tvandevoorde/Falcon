namespace Falcon.Application.Contracts.Logs;

/// <summary>
/// Represents a configured log pattern.
/// </summary>
public sealed class LogPatternDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Pattern { get; init; } = string.Empty;

    public string SeverityDefault { get; init; } = string.Empty;

    public bool Enabled { get; init; }
}
