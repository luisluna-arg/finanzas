using System.Text.Json.Serialization;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Frequencies;

public class GetFrequencyQueryHandler : BaseResponseHandler<GetFrequencyQuery, Frequency?>
{
    public GetFrequencyQueryHandler(
        FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<Frequency?> Handle(GetFrequencyQuery request, CancellationToken cancellationToken)
        => await this.DbContext.Frequency
            .FirstOrDefaultAsync(o => o.Id == request.Id);
}

public class GetFrequencyQuery : GetSingleByIdQuery<Frequency?, FrequencyEnum>
{
    [JsonIgnore]
    public override FrequencyEnum Id { get; set; } = default!;
}
