namespace FinanceApi.Models;

using System.ComponentModel.DataAnnotations.Schema;

public sealed class Movement : Entity, IEquatable<Movement>
{
    [ForeignKey("ModuleId")]
    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = Module.Default();
    public Currency? Currency { get; set; }
    public Guid? CurrencyId { get; set; } = null;
    required public DateTime TimeStamp { get; set; }
    required public string Concept1 { get; set; }
    required public string? Concept2 { get; set; }
    required public decimal Amount { get; set; }
    required public decimal? Total { get; set; }

    public override bool Equals(object? obj)
    {
        return this.Equals(obj as Movement);
    }

    public bool Equals(Movement? movement2)
    {
        if (movement2 is null) return false;

        var result =
            this.ModuleId == movement2.ModuleId &&
            this.TimeStamp == movement2.TimeStamp &&
            this.Amount == movement2.Amount &&
            this.Total == movement2.Total &&
            this.Concept1 == movement2.Concept1 &&
            this.Concept2 == movement2.Concept2;

        return result;
    }

    public override int GetHashCode() => (
        this.ModuleId,
        this.TimeStamp,
        this.Amount,
        this.Total,
        this.Concept1,
        this.Concept2).GetHashCode();
}
