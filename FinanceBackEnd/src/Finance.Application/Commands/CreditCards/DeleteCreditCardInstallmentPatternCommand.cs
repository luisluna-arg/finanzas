using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Application.Services;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Commands.CreditCards;

public sealed class DeleteCreditCardInstallmentPatternCommand : BaseDeleteCommand<Guid>;

public sealed class DeleteCreditCardInstallmentPatternCommandHandler(
    IEntityService<CreditCardInstallmentPattern, Guid> service)
    : BaseDeleteCommandHandler<DeleteCreditCardInstallmentPatternCommand, CreditCardInstallmentPattern, Guid>(service);

public sealed class DeleteCreditCardInstallmentPatternCommandValidator(
    IRepository<CreditCardInstallmentPattern, Guid> repository)
    : BaseDeleteCommandValidator<DeleteCreditCardInstallmentPatternCommand, CreditCardInstallmentPattern, Guid>(repository);
