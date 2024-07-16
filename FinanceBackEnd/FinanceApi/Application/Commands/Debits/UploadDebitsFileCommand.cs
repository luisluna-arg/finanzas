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
    private readonly IRepository<Debit, Guid> repository;
    private readonly IRepository<DebitOrigin, Guid> originRepository;
    private readonly DebitsExcelHelper excelHelper;

    public UploadDebitsFileCommandHandler(
        FinanceDbContext db,
        IAppModuleRepository appModuleRepository,
        IRepository<Debit, Guid> debitRepository,
        IRepository<DebitOrigin, Guid> debitOriginRepository)
        : base(db)
    {
        this.appModuleRepository = appModuleRepository;
        this.originRepository = debitOriginRepository;
        this.repository = debitRepository;
        this.excelHelper = new DebitsExcelHelper();
    }

    public override async Task Handle(UploadDebitsFileCommand command, CancellationToken cancellationToken)
    {
        var appModule = await appModuleRepository.GetByIdAsync(command.AppModuleId, cancellationToken);
        if (appModule == null) throw new Exception($"App module not found, Id: {command.AppModuleId}");

        var dateKind = command.DateKind;
        if (dateKind.Equals(DateTimeKind.Unspecified)) dateKind = DateTimeKind.Utc;

        var newRecords = excelHelper.Read(command.File, appModule, dateKind);
        if (newRecords == null || !newRecords.Any()) return;

        var minDate = newRecords.Min(o => o.TimeStamp);
        var maxDate = newRecords.Max(o => o.TimeStamp);

        var records = await repository.GetDbSet()
                .Include(o => o.Origin)
                .ToArrayAsync();

        var origins = new Dictionary<string, DebitOrigin>();

        foreach (var record in newRecords)
        {
            var existingRecord = records
                .FirstOrDefault(x => x.Origin.Name.Equals(record.Origin.Name, StringComparison.InvariantCultureIgnoreCase) &&
                    x.TimeStamp == record.TimeStamp);
            if (existingRecord != null) continue;

            var origin = origins.ContainsKey(record.Origin.Name) ?
                origins[record.Origin.Name] :
                await originRepository.GetByAsync("Name", record.Origin.Name, cancellationToken);

            if (origin != null)
            {
                record.Origin = origin;
            }
            else
            {
                origins.Add(record.Origin.Name, record.Origin);
            }

            await repository.AddAsync(record, cancellationToken, false);
        }

        var timeStampProperty = "TimeStamp";
        var existingRecords = repository
            .FilterBy(timeStampProperty, ExpressionOperator.GreaterThanOrEqual, minDate)
            .FilterBy(timeStampProperty, ExpressionOperator.LessThanOrEqual, maxDate)
            .Include(o => o.Origin)
            .Where(o => o.Origin.AppModuleId == appModule.Id)
            .ToArray();

        newRecords = newRecords
            .Where(o => existingRecords.All(x =>
                x.Origin.AppModuleId != o.Origin.AppModuleId ||
                x.OriginId != o.OriginId ||
                x.TimeStamp != o.TimeStamp ||
                x.Amount != o.Amount))
            .ToArray();

        await repository.AddRangeAsync(newRecords, cancellationToken, true);
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
