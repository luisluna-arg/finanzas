using Finance.Domain.SpecialTypes;

namespace Finance.Application.Dtos.CreditCards;

public record CreditCardStatementDraftDto
{
    public Guid CreditCardId { get; set; }
    public Guid CurrentStatementId { get; set; }
    public DateTime SuggestedClosureDate { get; set; }
    public DateTime SuggestedExpiringDate { get; set; }
    public List<CreditCardStatementDraftRowDto> ContinuingInstallments { get; set; } = [];
    public ICollection<CreditCardTransactionDto> EligibleNonPlanTransactions { get; set; } = [];
}

public record CreditCardStatementDraftRowDto
{
    public Guid PaymentPlanId { get; set; }
    public string BaseConcept { get; set; } = string.Empty;
    public int NextInstallmentNumber { get; set; }
    public int TotalInstallments { get; set; }
    public Money SuggestedAmount { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid SourceTransactionId { get; set; }
}
