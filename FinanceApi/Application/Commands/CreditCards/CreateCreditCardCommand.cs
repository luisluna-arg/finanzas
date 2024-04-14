using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.CreditCards;

public class CreateCreditCardCommandHandler : BaseResponseHandler<CreateCreditCardCommand, CreditCard>
{
    private readonly IRepository<CreditCard, Guid> creditCardRepository;
    private readonly IRepository<Bank, Guid> bankRepository;

    public CreateCreditCardCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository,
        IRepository<CreditCard, Guid> creditCardRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
        this.creditCardRepository = creditCardRepository;
    }

    public override async Task<CreditCard> Handle(CreateCreditCardCommand command, CancellationToken cancellationToken)
    {
        var bank = await bankRepository.GetByIdAsync(command.BankId, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        var newCreditCard = new CreditCard()
        {
            Bank = bank,
            Name = command.Name,
            Deactivated = command.Deactivated
        };

        await creditCardRepository.AddAsync(newCreditCard, cancellationToken);

        return await Task.FromResult(newCreditCard);
    }
}

public class CreateCreditCardCommand : IRequest<CreditCard>
{
    [Required]
    public Guid BankId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public bool Deactivated { get; set; }
}
