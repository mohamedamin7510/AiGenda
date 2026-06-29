namespace AI_genda_API.Abstractions;

public class PaginatedList<T>(List<T> Items, int PageNumber, int PageSize, int Count)
{
    public List<T> Items { get; } = Items;
    public int PageNumber { get; } = PageNumber;
    public int PageSize { get; } = PageSize;
    public int TotalPages { get; } = (int)Math.Ceiling(Count / (double)PageSize);


    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber  < TotalPages;


    public static async Task<PaginatedList<T>> CreateAsync( IQueryable<T> items, int pageNumber, int pageSize)
    {
        var count = await items.CountAsync();

        var resultItems = await items.Skip((pageNumber - 1 ) * pageSize ).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(resultItems, pageNumber, pageSize, count);
    }

}
