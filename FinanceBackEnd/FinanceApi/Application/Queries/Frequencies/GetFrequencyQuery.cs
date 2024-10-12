using System.Text.Json.Serialization;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Frequencies;

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
