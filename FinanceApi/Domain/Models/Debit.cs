using System.ComponentModel.DataAnnotations.Schema;
using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public sealed class Debit : Entity<Guid>, IEquatable<Debit>
{
    [ForeignKey("DebitOriginId")]
    public Guid DebitOriginId { get; set; }

    public DebitOrigin DebitOrigin { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public Money Amount { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Debit);
    }

    public bool Equals(Debit? debit)
    {
        if (debit is null) return false;

        return
            DebitOriginId == debit.DebitOriginId &&
            TimeStamp == debit.TimeStamp &&
            Amount == debit.Amount;
    }

    public override int GetHashCode() => (
        DebitOriginId,
        TimeStamp,
        Amount).GetHashCode();
}
