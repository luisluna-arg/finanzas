namespace Finance.Application.Services.CreditCards;

public sealed record CreateCreditCardRequest(Guid BankId, string Name, bool Deactivated);

public sealed record UpdateCreditCardRequest(Guid Id, Guid BankId, string Name, bool Deactivated);

public sealed record DeleteCreditCardRequest(Guid[] Ids);
