using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Helpers.ExcelHelper;
using Finance.Application.Repositories;
using Finance.Application.Repositories.Base;
using Finance.Persistance;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.CreditCards;

public class UploadCreditCardFileCommandHandler : BaseResponselessHandler<UploadCreditCardFileCommand>
{
    private readonly IRepository<CreditCardMovement, Guid> repository;
    private readonly IRepository<CreditCard, Guid> issuerRepository;
    private readonly CreditCardExcelHelper excelHelper;

    public UploadCreditCardFileCommandHandler(
        FinanceDbContext db,
        IRepository<CreditCardMovement, Guid> debitRepository,
        IRepository<CreditCard, Guid> creditCardRepository)
        : base(db)
    {
        this.issuerRepository = creditCardRepository;
        this.repository = debitRepository;
        this.excelHelper = new CreditCardExcelHelper();
    }

    public override async Task Handle(UploadCreditCardFileCommand command, CancellationToken cancellationToken)
    {
        var issuer = await issuerRepository.GetByIdAsync(command.CreditCardId, cancellationToken);
        if (issuer == null) throw new Exception($"Credit Card Issuer not found, Id: {command.CreditCardId}");

        var dateKind = command.DateKind;
        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;

        var newRecords = excelHelper.Read(command.File, issuer, dateKind);
        if (newRecords == null || !newRecords.Any()) return;

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var timeStampProperty = "TimeStamp";
        var existingRecords = repository
            .FilterBy(timeStampProperty, ExpressionOperator.GreaterThanOrEqual, minDate)
            .FilterBy(timeStampProperty, ExpressionOperator.LessThanOrEqual, maxDate)
            .Include(o => o.CreditCard)
            .Where(o => o.CreditCardId == issuer.Id)
            .ToArray();

        newRecords = newRecords
            .Where(o => existingRecords.All(x =>
                x.CreditCardId != o.CreditCard.Id ||
                x.TimeStamp != o.TimeStamp ||
                x.Concept != o.Concept ||
                x.Amount != o.Amount ||
                x.AmountDollars != o.AmountDollars))
            .ToArray();

        await repository.AddRangeAsync(newRecords, cancellationToken, true);
    }
}

public class UploadCreditCardFileCommand : IRequest
{
    public UploadCreditCardFileCommand(IFormFile file, string creditCardId, DateTimeKind dateKind)
    {
        this.File = file;
        this.CreditCardId = new Guid(creditCardId);
        this.DateKind = dateKind;
    }

    public IFormFile File { get; set; }
    public Guid CreditCardId { get; set; }
    public DateTimeKind DateKind { get; set; }
}
