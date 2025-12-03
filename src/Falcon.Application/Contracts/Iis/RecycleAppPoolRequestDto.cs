namespace Falcon.Application.Contracts.Iis;

/// <summary>
/// Represents a request to recycle an IIS application pool.
/// </summary>
public sealed class RecycleAppPoolRequestDto
{
    public string? Reason { get; init; }
}