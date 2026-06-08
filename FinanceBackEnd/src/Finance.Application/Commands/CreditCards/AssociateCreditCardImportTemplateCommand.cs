using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.CreditCards;

public class AssociateCreditCardImportTemplateCommandHandler : BaseResponselessHandler<AssociateCreditCardImportTemplateCommand>
{
    public AssociateCreditCardImportTemplateCommandHandler(FinanceDbContext db) : base(db) { }

    public override async Task<CommandResult> ExecuteAsync(
        AssociateCreditCardImportTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await DbContext.CreditCardStatementImportTemplate
            .IgnoreQueryFilters()
            .Include(t => t.CreditCards)
            .FirstOrDefaultAsync(t => t.Id == command.TemplateId, cancellationToken);

        if (template is null)
            return CommandResult.Failure($"Template not found: {command.TemplateId}");

        if (template.CreditCards.Any(c => c.Id == command.CreditCardId))
            return CommandResult.Success();

        var creditCard = await DbContext.CreditCard
            .FirstOrDefaultAsync(c => c.Id == command.CreditCardId, cancellationToken);

        if (creditCard is null)
            return CommandResult.Failure($"Credit card not found: {command.CreditCardId}");

        template.CreditCards.Add(creditCard);
        await DbContext.SaveChangesAsync(cancellationToken);

        return CommandResult.Success();
    }
}

public class AssociateCreditCardImportTemplateCommand : ICommand
{
    public Guid TemplateId { get; set; }
    public Guid CreditCardId { get; set; }
}
