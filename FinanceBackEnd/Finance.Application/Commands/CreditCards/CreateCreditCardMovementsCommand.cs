using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.CreditCards;

public class CreateCreditCardMovementCommandHandler : BaseResponseHandler<CreateCreditCardMovementCommand, CreditCardMovement>
{
    private readonly IRepository<CreditCard, Guid> creditCardRepository;
    private readonly IRepository<CreditCardMovement, Guid> creditCardMovementRepository;

    public CreateCreditCardMovementCommandHandler(
        FinanceDbContext db,
        IRepository<CreditCard, Guid> creditCardRepository,
        IRepository<CreditCardMovement, Guid> creditCardMovementRepository)
        : base(db)
    {
        this.creditCardRepository = creditCardRepository;
        this.creditCardMovementRepository = creditCardMovementRepository;
    }

    public override async Task<CreditCardMovement> Handle(CreateCreditCardMovementCommand command, CancellationToken cancellationToken)
    {
        var creditCard = await creditCardRepository.GetByIdAsync(command.CreditCardId, cancellationToken);
        if (creditCard == null) throw new Exception($"Credit Card not found, Id: {command.CreditCardId}");

        var newCreditCardMovement = new CreditCardMovement()
        {
            CreditCard = creditCard,
            TimeStamp = command.TimeStamp,
            PlanStart = command.PlanStart,
            Concept = command.Concept,
            PaymentNumber = command.PaymentNumber,
            PlanSize = command.PlanSize,
            Amount = command.Amount,
            AmountDollars = command.AmountDollars,
        };

        await creditCardMovementRepository.AddAsync(newCreditCardMovement, cancellationToken);

        return await Task.FromResult(newCreditCardMovement);
    }
}

public class CreateCreditCardMovementCommand : IRequest<CreditCardMovement>
{
    public Guid CreditCardId { get; set; }
    public DateTime TimeStamp { get; set; }
    public DateTime PlanStart { get; set; }
    public string Concept { get; set; } = string.Empty;
    public ushort PaymentNumber { get; set; }
    public ushort PlanSize { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountDollars { get; set; }
}
