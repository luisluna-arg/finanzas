using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.CreditCards;

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
        var bank = await bankRepository.GetById(command.BankId);
        if (bank == null) throw new Exception("Bank not found");

        var creditCard = await creditCardRepository.GetById(command.Id);
        if (creditCard == null) throw new Exception("Credit Card Issuer not found");

        creditCard.Bank = bank;
        creditCard.Name = command.Name;
        creditCard.Deactivated = command.Deactivated;

        await creditCardRepository.Update(creditCard);
        await creditCardRepository.Persist();

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
