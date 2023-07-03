using FinanceApi.Application.Commands.Funds;
using FinanceApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Funds;

public class UploadImageCommandHandler : BaseResponselessHandler<UploadImageCommand>
{
    public UploadImageCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task Handle(UploadImageCommand command, CancellationToken cancellationToken)
    {
        var appModule = await DbContext.AppModule.FirstOrDefaultAsync(o => o.Name == "Fondos");
        if (appModule == null) throw new Exception("Fund app module not found");
    }
}
