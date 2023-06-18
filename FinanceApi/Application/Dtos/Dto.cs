namespace FinanceApi.Application.Dtos;

public abstract record Dto
{
    protected Dto()
    {
    }

    public Guid Id { get; set; }
}
