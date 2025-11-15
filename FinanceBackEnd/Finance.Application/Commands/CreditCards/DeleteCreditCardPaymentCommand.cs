using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.CreditCards;
using Finance.Persistence;

namespace Finance.Application.Commands.CreditCards;

public class DeleteCreditCardPaymentCommandHandler : BaseCommandHandler<DeleteCreditCardPaymentCommand, CreditCardPayment>
{
    private readonly IRepository<CreditCardPayment, Guid> paymentRepository;

    public DeleteCreditCardPaymentCommandHandler(
        FinanceDbContext db,
        IRepository<CreditCardPayment, Guid> paymentRepository)
        : base(db)
    {
        this.paymentRepository = paymentRepository;
    }

    public override async Task<DataResult<CreditCardPayment>> ExecuteAsync(DeleteCreditCardPaymentCommand command, CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (payment == null) throw new Exception("Credit card payment not found");

        payment.Deactivated = true;
        await paymentRepository.UpdateAsync(payment, cancellationToken);

        return DataResult<CreditCardPayment>.Success(payment);
    }
}

public class DeleteCreditCardPaymentCommand : ICommand
{
    [Required]
    public Guid Id { get; set; }
}
