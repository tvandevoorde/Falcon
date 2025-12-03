namespace Falcon.Application.Contracts.Common;

/// <summary>
/// Represents a paged response payload for API consumers.
/// </summary>
/// <typeparam name="T">Type of the items contained in the response.</typeparam>
public sealed class PagedResponseDto<T>(int total, IReadOnlyCollection<T> items)
{
    public int Total { get; } = total;

    public IReadOnlyCollection<T> Items { get; } = items;
}