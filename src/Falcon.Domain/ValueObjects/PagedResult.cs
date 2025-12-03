namespace Falcon.Domain.ValueObjects;

/// <summary>
/// Represents a paged result set returned by repository queries.
/// </summary>
/// <typeparam name="T">Type of items contained in the page.</typeparam>
public sealed class PagedResult<T>(int total, IReadOnlyCollection<T> items)
{
    public int Total { get; } = total;

    public IReadOnlyCollection<T> Items { get; } = items;
}