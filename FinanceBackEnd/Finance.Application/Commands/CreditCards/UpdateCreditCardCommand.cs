using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.CreditCards;

public class UpdateCreditCardCommandHandler : BaseResponseHandler<UpdateCreditCardCommand, CreditCard>
{
    private readonly IRepository<CreditCard, Guid> creditCardRepository;
    private readonly IRepository<Bank, Guid> bankRepository;

    public UpdateCreditCardCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository,
        IRepository<CreditCard, Guid> creditCardRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
        this.creditCardRepository = creditCardRepository;
    }

    public override async Task<CreditCard> Handle(UpdateCreditCardCommand command, CancellationToken cancellationToken)
    {
        var bank = await bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        var creditCard = await creditCardRepository.GetByIdAsync(command.Id, cancellationToken);
        if (creditCard == null) throw new Exception("Credit Card Issuer not found");

        creditCard.Bank = bank;
        creditCard.Name = command.Name;
        creditCard.Deactivated = command.Deactivated;

        await creditCardRepository.UpdateAsync(creditCard, cancellationToken);
        await creditCardRepository.PersistAsync(cancellationToken);

        return await Task.FromResult(creditCard);
    }
}

public class UpdateCreditCardCommand : IRequest<CreditCard>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid BankId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public bool Deactivated { get; set; }
}
