namespace Finance.Application.Dtos.Summary;

public class BaseSummaryTotals<TItem>
    where TItem : BaseSummaryItem
{
    protected List<TItem> items;

    protected BaseSummaryTotals()
    {
        items = [];
    }

    protected BaseSummaryTotals(ICollection<TItem> items)
    {
        this.items = [.. items];
    }

    public List<TItem> Items { get => items; }

    public List<TItem> Add(TItem item)
    {
        this.items.Add(item);
        this.items = this.items.OrderBy(o => o.Label).ToList();
        return Items;
    }

    public List<TItem> AddRange(ICollection<TItem> items)
    {
        this.items.AddRange(items);
        this.items = this.items.OrderBy(o => o.Label).ToList();
        return Items;
    }
}