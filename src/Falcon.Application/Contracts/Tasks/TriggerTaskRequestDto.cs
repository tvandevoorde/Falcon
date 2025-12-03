namespace Falcon.Application.Contracts.Tasks;

/// <summary>
/// Represents a manual task trigger request.
/// </summary>
public sealed class TriggerTaskRequestDto
{
    public string? Reason { get; init; }
}