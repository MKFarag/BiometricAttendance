namespace BiometricAttendance.Infrastructure.Persistence.Common;

/// <summary>Represents a paginated list of items of type <typeparamref name="T"/></summary>
/// <typeparam name="T">The type of elements in the list</typeparam>
/// <param name="items">The items to include in the current page</param>
/// <param name="pageNumber">The current page number (1-based)</param>
/// <param name="count">The total number of items in the source collection</param>
/// <param name="pageSize">The number of items per page</param>
public class PaginatedList<T> : IPaginatedList<T> where T : class
{
    public List<T> Items { get; private set; }
    public int PageNumber { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList(List<T> items, int pageNumber, int count, int pageSize)
    {
        Items = items;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    private PaginatedList(List<T> items, int pageNumber, int totalPages)
    {
        Items = items;
        PageNumber = pageNumber;
        TotalPages = totalPages;
    }

    /// <summary>
    /// Asynchronously creates a paginated list from the given IQueryable source.
    /// </summary>
    /// <param name="source">The IQueryable data source to paginate.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A PaginatedList containing the items for the specified page.</returns>
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, pageNumber, count, pageSize);
    }

    /// <summary>
    /// Creates a new PaginatedList<T> containing the specified items and copies the page number and total pages from an
    /// existing paginated list.
    /// </summary>
    /// <typeparam name="Old">The element type of the provided existing paginated list.</typeparam>
    /// <param name="newItems">The items for the new paginated list.</param>
    /// <param name="exitingList">The paginated list to copy page number and total pages from.</param>
    /// <returns>A PaginatedList<T> containing newItems and the pagination metadata (page number and total pages) from
    /// exitingList.</returns>
    public static PaginatedList<T> CopyWithNewItems<Old>(List<T> newItems, PaginatedList<Old> exitingList) where Old : class
    {
        return new PaginatedList<T>(newItems, exitingList.PageNumber, exitingList.TotalPages);
    }
}
