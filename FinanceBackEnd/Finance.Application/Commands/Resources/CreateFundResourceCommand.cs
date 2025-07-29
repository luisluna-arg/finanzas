using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class CreateFundResourceCommand : CreateEntityResourceCommand<Fund, Guid, FundResource>
{
    public Guid FundId { get; set; }
}

public class CreateFundResourceCommandHandler(FinanceDbContext dbContext)
    : CreateEntityResourceCommandHandler<CreateFundResourceCommand, Fund, Guid, FundResource>(dbContext)
{
    protected override async Task<Fund?> QuerySource(CreateFundResourceCommand request, CancellationToken cancellationToken)
    {
        return await DbContext.Fund
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(f => f.Id == request.FundId, cancellationToken);
    }
}
