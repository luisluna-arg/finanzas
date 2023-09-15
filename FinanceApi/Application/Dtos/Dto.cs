namespace FinanceApi.Application.Dtos;

public abstract record Dto<TId> : IDto
{
    protected Dto()
    {
    }

    public TId Id { get; set; } = default!;
}
