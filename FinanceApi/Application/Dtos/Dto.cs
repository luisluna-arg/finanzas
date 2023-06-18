namespace FinanceApi.Application.Dtos;

public abstract record Dto
{
    public Dto()
    {
    }

    public Guid Id { get; set; }
}
