using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public sealed class CreditCardMovement : Entity<Guid>, IEquatable<CreditCardMovement>
{
    public Guid CreditCardIssuerId { get; set; }
    public CreditCardIssuer CreditCardIssuer { get; set; }
    required public DateTime TimeStamp { get; set; }
    required public DateTime PlanStart { get; set; }
    required public string Concept { get; set; }
    required public ushort PaymentNumber { get; set; } = 1;
    required public ushort PlanSize { get; set; } = 1;
    required public Money Amount { get; set; } = 0;
    required public Money AmountDollars { get; set; } = 0;

    public override bool Equals(object? obj)
    {
        return Equals(obj as CreditCardMovement);
    }

    public bool Equals(CreditCardMovement? debit)
    {
        if (debit is null) return false;

        return
            CreditCardIssuerId == debit.CreditCardIssuerId &&
            TimeStamp == debit.TimeStamp &&
            PlanStart == debit.PlanStart &&
            Concept == debit.Concept &&
            PaymentNumber == debit.PaymentNumber &&
            PlanSize == debit.PlanSize &&
            Amount == debit.Amount &&
            AmountDollars == debit.AmountDollars;
    }

    public override int GetHashCode() => (
        CreditCardIssuerId,
        TimeStamp,
        PlanStart,
        Concept,
        PaymentNumber,
        PlanSize,
        Amount,
        AmountDollars).GetHashCode();
}
