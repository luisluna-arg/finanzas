using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class DeleteCreditCardOwnerCommand
    : DeleteEntityOwnerCommand<CreditCard, Guid, CreditCardPermissions>;

public class DeleteCreditCardOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteCreditCardOwnerCommand, CreditCard, Guid, CreditCardPermissions>(dbContext);
