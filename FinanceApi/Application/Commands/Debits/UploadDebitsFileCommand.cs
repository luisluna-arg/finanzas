using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Helpers.ExcelHelper;
using FinanceApi.Infrastructure.Repositories;
using FinanceApi.Infrastructure.Repositories.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Commands.Debits;

public class UploadDebitsFileCommandHandler : BaseResponselessHandler<UploadDebitsFileCommand>
{
    private readonly IAppModuleRepository appModuleRepository;
    private readonly IRepository<Debit, Guid> debitRepository;
    private readonly DebitsExcelHelper excelHelper;

    public UploadDebitsFileCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<Debit, Guid> debitRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
        this.debitRepository = debitRepository;
        this.excelHelper = new DebitsExcelHelper();
    }

    public override async Task Handle(UploadDebitsFileCommand command, CancellationToken cancellationToken)
    {
        var appModule = await appModuleRepository.GetFunds();

        var dateKind = command.DateKind;
        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;

        var newRecords = excelHelper.ReadAsync(command.File, appModule, dateKind);
        if (newRecords == null || !newRecords.Any()) return;

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var timeStampProperty = "TimeStamp";
        var existingRecords = debitRepository
            .FilterBy(timeStampProperty, ExpressionOperator.GreaterThanOrEqual, minDate)
            .FilterBy(timeStampProperty, ExpressionOperator.LessThanOrEqual, maxDate)
            .Include(o => o.DebitOrigin)
            .Where(o => o.DebitOrigin.AppModuleId == appModule.Id)
            .ToArray();

        newRecords = newRecords
            .Where(o => existingRecords.All(x =>
                x.DebitOrigin.AppModuleId != o.DebitOrigin.AppModuleId ||
                x.DebitOriginId != o.DebitOriginId ||
                x.TimeStamp != o.TimeStamp ||
                x.Amount != o.Amount))
            .ToArray();

        await debitRepository.AddRange(newRecords, true);
    }
}

public class UploadDebitsFileCommand : IRequest
{
    public UploadDebitsFileCommand(IFormFile file, string appModuleId, DateTimeKind dateKind)
    {
        this.File = file;
        this.AppModuleId = new Guid(appModuleId);
        this.DateKind = dateKind;
    }

    public IFormFile File { get; set; }
    public Guid AppModuleId { get; set; }
    public DateTimeKind DateKind { get; set; }
}
