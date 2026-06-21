using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class SetCreditCardDefaultImportTemplateCommandHandler : BaseResponselessHandler<SetCreditCardDefaultImportTemplateCommand>
{
    private readonly IRepository<CreditCard, Guid> _creditCardRepository;

    public SetCreditCardDefaultImportTemplateCommandHandler(
        FinanceDbContext db,
        IRepository<CreditCard, Guid> creditCardRepository)
        : base(db)
    {
        _creditCardRepository = creditCardRepository;
    }

    public override async Task<CommandResult> ExecuteAsync(
        SetCreditCardDefaultImportTemplateCommand command, CancellationToken cancellationToken)
    {
        var creditCard = await _creditCardRepository.GetByIdAsync(command.CreditCardId, cancellationToken)
            ?? throw new Exception($"Credit card not found: {command.CreditCardId}");

        creditCard.DefaultImportTemplateId = command.TemplateId;
        await _creditCardRepository.UpdateAsync(creditCard, cancellationToken);
        await _creditCardRepository.PersistAsync(cancellationToken);

        return CommandResult.Success();
    }
}

public class SetCreditCardDefaultImportTemplateCommand : ICommand
{
    public Guid CreditCardId { get; set; }
    public Guid? TemplateId { get; set; }
}
