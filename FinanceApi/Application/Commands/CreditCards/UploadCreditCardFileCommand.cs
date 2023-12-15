using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Helpers.ExcelHelper;
using FinanceApi.Infrastructure.Repositories;
using FinanceApi.Infrastructure.Repositories.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Commands.CreditCards;

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
        var issuer = await issuerRepository.GetById(command.IssuerId);
        if (issuer == null) throw new Exception($"Credit Card Issuer not found, Id: {command.IssuerId}");

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

        await repository.AddRange(newRecords, true);
    }
}

public class UploadCreditCardFileCommand : IRequest
{
    public UploadCreditCardFileCommand(IFormFile file, string issuerId, DateTimeKind dateKind)
    {
        this.File = file;
        this.IssuerId = new Guid(issuerId);
        this.DateKind = dateKind;
    }

    public IFormFile File { get; set; }
    public Guid IssuerId { get; set; }
    public DateTimeKind DateKind { get; set; }
}
