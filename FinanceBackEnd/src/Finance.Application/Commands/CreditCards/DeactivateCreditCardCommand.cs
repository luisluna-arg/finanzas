using Finance.Application.Commands.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class DeactivateCreditCardCommand : BatchUpdateBaseCommand;

public class DeactivateCreditCardCommandHandler(FinanceDbContext dbContext)
    : BaseDeactivateCommandHandler<DeactivateCreditCardCommand, DeactivateCreditCardCommandValidator, CreditCard, Guid>(dbContext);

public class DeactivateCreditCardCommandValidator : BatchUpdateBaseCommandValidator<DeactivateCreditCardCommand>;
