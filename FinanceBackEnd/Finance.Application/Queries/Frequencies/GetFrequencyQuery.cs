using System.Text.Json.Serialization;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Frequencies;

public class GetFrequencyQuery : GetSingleByIdQuery<Frequency?, FrequencyEnum>
{
    [JsonIgnore]
    public override FrequencyEnum Id { get; set; } = default!;
}

public class GetFrequencyQueryHandler(FinanceDbContext db) : BaseResponseHandler<GetFrequencyQuery, Frequency?>(db)
{
    public override async Task<Frequency?> Handle(GetFrequencyQuery request, CancellationToken cancellationToken)
        => await DbContext.Frequency.FirstOrDefaultAsync(o => o.Id == request.Id);
}
