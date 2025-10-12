using System.Text.Json.Serialization;
using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Frequencies;

public class GetFrequencyQuery : GetSingleByIdQuery<Frequency?, FrequencyEnum>
{
    [JsonIgnore]
    public override FrequencyEnum Id { get; set; } = default!;
}

public class GetFrequencyQueryHandler(FinanceDbContext db) : BaseQueryHandler<GetFrequencyQuery, Frequency?>(db)
{
    public override async Task<DataResult<Frequency?>> ExecuteAsync(GetFrequencyQuery request, CancellationToken cancellationToken)
        => DataResult<Frequency?>.Success(await DbContext.Frequency.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken));
}
