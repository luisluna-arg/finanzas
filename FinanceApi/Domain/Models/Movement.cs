using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceApi.Domain.Models;

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
        return Equals(obj as Movement);
    }

    public bool Equals(Movement? movement2)
    {
        if (movement2 is null) return false;

        var result =
            ModuleId == movement2.ModuleId &&
            TimeStamp == movement2.TimeStamp &&
            Amount == movement2.Amount &&
            Total == movement2.Total &&
            Concept1 == movement2.Concept1 &&
            Concept2 == movement2.Concept2;

        return result;
    }

    public override int GetHashCode() => (
        ModuleId,
        TimeStamp,
        Amount,
        Total,
        Concept1,
        Concept2).GetHashCode();
}
