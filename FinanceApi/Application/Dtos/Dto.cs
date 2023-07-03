namespace FinanceApi.Application.Dtos;

public abstract record Dto<TId>
{
    protected Dto()
    {
    }

    public TId Id { get; set; } = default!;
}
