using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Helpers.ExcelHelper;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.CreditCards;

public class UploadCreditCardTransactionFileCommandHandler : BaseResponselessHandler<UploadCreditCardTransactionFileCommand>
{
    private readonly IRepository<CreditCardTransaction, Guid> transactionRepository;
    private readonly IRepository<CreditCard, Guid> creditCardRepository;
    private readonly CreditCardTransactionExcelHelper excelHelper;

    public UploadCreditCardTransactionFileCommandHandler(
        FinanceDbContext db,
        IRepository<CreditCardTransaction, Guid> transactionRepository,
        IRepository<CreditCard, Guid> creditCardRepository)
        : base(db)
    {
        this.creditCardRepository = creditCardRepository;
        this.transactionRepository = transactionRepository;
        this.excelHelper = new CreditCardTransactionExcelHelper();
    }

    public override async Task<CommandResult> ExecuteAsync(UploadCreditCardTransactionFileCommand command, CancellationToken cancellationToken)
    {
        var creditCard = await creditCardRepository.GetByIdAsync(command.CreditCardId, cancellationToken);
        if (creditCard == null) throw new Exception($"Credit Card not found, Id: {command.CreditCardId}");

        var dateKind = command.DateKind;
        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;

        var newRecords = excelHelper.Read(command.File, creditCard, dateKind);
        if (newRecords == null || !newRecords.Any()) return CommandResult.Failure("No records found in the uploaded file.");

        var minDate = newRecords.Min(o => o.Timestamp);
        var maxDate = newRecords.Max(o => o.Timestamp);

        var timestampProperty = "Timestamp";
        var existingRecords = transactionRepository
            .FilterBy(timestampProperty, ExpressionOperator.GreaterThanOrEqual, minDate)
            .FilterBy(timestampProperty, ExpressionOperator.LessThanOrEqual, maxDate)
            .Include(o => o.CreditCard)
            .Where(o => o.CreditCardId == creditCard.Id)
            .ToArray();

        newRecords = newRecords
            .Where(o => existingRecords.All(x =>
                x.CreditCardId != o.CreditCard.Id ||
                x.Timestamp != o.Timestamp ||
                x.Concept != o.Concept ||
                x.Amount != o.Amount))
            .ToArray();

        await transactionRepository.AddRangeAsync(newRecords, cancellationToken, true);

        return CommandResult.Success();
    }
}

public class UploadCreditCardTransactionFileCommand : ICommand
{
    public UploadCreditCardTransactionFileCommand(IFormFile file, string creditCardId, DateTimeKind dateKind)
    {
        this.File = file;
        this.CreditCardId = new Guid(creditCardId);
        this.DateKind = dateKind;
    }

    public IFormFile File { get; set; }
    public Guid CreditCardId { get; set; }
    public DateTimeKind DateKind { get; set; }
}
