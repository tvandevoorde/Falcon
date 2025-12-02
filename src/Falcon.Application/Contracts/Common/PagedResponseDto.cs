namespace Falcon.Application.Contracts.Common;

/// <summary>
/// Represents a paged response payload for API consumers.
/// </summary>
/// <typeparam name="T">Type of the items contained in the response.</typeparam>
public sealed class PagedResponseDto<T>
{
    public PagedResponseDto(int total, IReadOnlyCollection<T> items)
    {
        Total = total;
        Items = items;
    }

    public int Total { get; }

    public IReadOnlyCollection<T> Items { get; }
}