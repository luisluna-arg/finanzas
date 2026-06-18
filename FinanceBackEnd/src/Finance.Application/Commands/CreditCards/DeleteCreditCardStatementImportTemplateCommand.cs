using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Application.Services;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Commands.CreditCards;

public sealed class DeleteCreditCardStatementImportTemplateCommand : BaseDeleteCommand<Guid>;

public sealed class DeleteCreditCardStatementImportTemplateCommandHandler(
    IEntityService<CreditCardStatementImportTemplate, Guid> service)
    : BaseDeleteCommandHandler<DeleteCreditCardStatementImportTemplateCommand, CreditCardStatementImportTemplate, Guid>(service);

public sealed class DeleteCreditCardStatementImportTemplateCommandValidator(
    IRepository<CreditCardStatementImportTemplate, Guid> repository)
    : BaseDeleteCommandValidator<DeleteCreditCardStatementImportTemplateCommand, CreditCardStatementImportTemplate, Guid>(repository);
