namespace Finance.Application.Dtos;

public abstract record Dto<TId> : IDto
{
    protected Dto()
    {
    }

    public TId Id { get; set; } = default!;

    public bool Deactivated { get; set; }
}
