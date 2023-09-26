using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public sealed class CreditCardIssuer : Entity<Guid>, IEquatable<CreditCardIssuer>
{
    public Guid BankId { get; set; }

    public Bank Bank { get; set; }

    required public string Name { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CreditCardIssuer);
    }

    public bool Equals(CreditCardIssuer? origin)
    {
        if (origin is null) return false;

        return BankId == origin.BankId &&
            Name == origin.Name;
    }

    public override int GetHashCode() => (
        BankId,
        Name).GetHashCode();
}
