using FinanceApi.Domain.Models.Interfaces;

namespace FinanceApi.Commons;

public class PaginatedResult<T> : IEntity
{
    public PaginatedResult(IEnumerable<T> items, int page, int pageSize, int totalItems)
    {
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
        Items = items;
        HasMore = (Page * PageSize) < TotalItems;
    }

    public int Page { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasMore { get; }
    public IEnumerable<T> Items { get; }

    public void Update(IEntity newData)
    {
        throw new NotImplementedException();
    }
}
