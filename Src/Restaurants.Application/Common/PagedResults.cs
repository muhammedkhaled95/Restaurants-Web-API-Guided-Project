namespace Restaurants.Application.Common;

public class PagedResults<T>
{
    public PagedResults(IEnumerable<T> items, int totalcount, int pageSize, int pageNumber)
    {
        Items = items;
        TotalItemsCount = totalcount;
        TotalPagesCount = (int)Math.Ceiling(TotalItemsCount / (double)pageSize);
        ItemsFrom = pageSize * (pageNumber - 1) + 1;
        // ItemsTo is done that way to account for an edge case where the last page is not a full sized page
        ItemsTo = Math.Min(ItemsFrom + pageSize - 1, TotalItemsCount);
    }
    public IEnumerable<T> Items { get; set; }
    public int TotalPagesCount { get; set; }
    public int TotalItemsCount { get; set; }
    public int ItemsFrom { get; set; }
    public int ItemsTo { get; set; }
}
