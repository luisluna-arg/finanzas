using System.ComponentModel.DataAnnotations.Schema;
using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public sealed class Movement : Entity<Guid>, IEquatable<Movement>
{
    [ForeignKey("AppModuleId")]
    public Guid AppModuleId { get; set; }

    public AppModule AppModule { get; set; }

    public Bank? Bank { get; set; }

    public Currency? Currency { get; set; }

    public Guid? CurrencyId { get; set; } = null;

    required public DateTime TimeStamp { get; set; }

    required public string Concept1 { get; set; }

    required public string? Concept2 { get; set; }

    required public Money Amount { get; set; }

    required public Money? Total { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Movement);
    }

    public bool Equals(Movement? movement2)
    {
        if (movement2 is null) return false;

        var result =
            AppModuleId == movement2.AppModuleId &&
            TimeStamp == movement2.TimeStamp &&
            Amount == movement2.Amount &&
            Total == movement2.Total &&
            Concept1 == movement2.Concept1 &&
            Concept2 == movement2.Concept2;

        return result;
    }

    public override int GetHashCode() => (
        AppModuleId,
        TimeStamp,
        Amount,
        Total,
        Concept1,
        Concept2).GetHashCode();
}
