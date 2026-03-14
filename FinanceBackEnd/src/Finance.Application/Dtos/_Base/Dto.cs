namespace Finance.Application.Dtos.Base;

public abstract record Dto<TId> : IDto
{
    public Dto() { }

    public TId Id { get; set; } = default!;
    public bool Deactivated { get; set; }
}
