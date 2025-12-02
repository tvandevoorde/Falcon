namespace Falcon.Domain.ValueObjects;

/// <summary>
/// Represents a paged result set returned by repository queries.
/// </summary>
/// <typeparam name="T">Type of items contained in the page.</typeparam>
public sealed class PagedResult<T>
{
    public PagedResult(int total, IReadOnlyCollection<T> items)
    {
        Total = total;
        Items = items;
    }

    public int Total { get; }

    public IReadOnlyCollection<T> Items { get; }
}