using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class CreateCreditCardPaymentCommandHandler : BaseCommandHandler<CreateCreditCardPaymentCommand, CreditCardPayment>
{
    private readonly IRepository<CreditCardPayment, Guid> paymentRepository;
    private readonly IRepository<CreditCardStatement, Guid> statementRepository;

    public CreateCreditCardPaymentCommandHandler(
        FinanceDbContext db,
        IRepository<CreditCardStatement, Guid> statementRepository,
        IRepository<CreditCardPayment, Guid> paymentRepository)
        : base(db)
    {
        this.statementRepository = statementRepository;
        this.paymentRepository = paymentRepository;
    }

    public override async Task<DataResult<CreditCardPayment>> ExecuteAsync(CreateCreditCardPaymentCommand command, CancellationToken cancellationToken)
    {
        var statement = await statementRepository.GetByIdAsync(command.StatementId, cancellationToken);
        if (statement == null) throw new Exception("Credit card statement not found");

        var newPayment = new CreditCardPayment()
        {
            StatementId = command.StatementId,
            Timestamp = command.Timestamp,
            Amount = command.Amount,
            Method = command.Method,
            Status = command.Status,
            Deactivated = command.Deactivated
        };

        await paymentRepository.AddAsync(newPayment, cancellationToken);

        return DataResult<CreditCardPayment>.Success(newPayment);
    }
}

public class CreateCreditCardPaymentCommand : ICommand
{
    [Required]
    public Guid StatementId { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Required]
    public Money Amount { get; set; } = 0;

    public PaymentMethod Method { get; set; } = PaymentMethod.Other;

    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

    public bool Deactivated { get; set; } = false;
}
