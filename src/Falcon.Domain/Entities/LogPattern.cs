namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a log pattern used to detect known issues.
/// </summary>
public sealed class LogPattern(Guid id, string name, string pattern, string severityDefault, bool enabled)
{
    public Guid Id { get; } = id;

    public string Name { get; private set; } = name;

    public string Pattern { get; private set; } = pattern;

    public string SeverityDefault { get; private set; } = severityDefault;

    public bool Enabled { get; private set; } = enabled;

    /// <summary>
    /// Updates the log pattern definition.
    /// </summary>
    /// <param name="name">Friendly name.</param>
    /// <param name="pattern">Regular expression or pattern.</param>
    /// <param name="severityDefault">Default severity label.</param>
    /// <param name="enabled">Enabled flag.</param>
    public void Update(string name, string pattern, string severityDefault, bool enabled)
    {
        Name = name;
        Pattern = pattern;
        SeverityDefault = severityDefault;
        Enabled = enabled;
    }
}
