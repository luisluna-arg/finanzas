using Finance.Application.Commands.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class ActivateCreditCardCommand : BatchUpdateBaseCommand;

public class ActivateCreditCardCommandHandler(FinanceDbContext dbContext)
    : BaseActivateCommandHandler<ActivateCreditCardCommand, ActivateCreditCardCommandValidator, CreditCard, Guid>(dbContext);

public class ActivateCreditCardCommandValidator : BatchUpdateBaseCommandValidator<ActivateCreditCardCommand>;
