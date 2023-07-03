using FinanceApi.Application.Commands.Banks;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;

namespace FinanceApi.Application.Handlers.Banks;

public class CreateBankCommandHandler : BaseResponseHandler<CreateBankCommand, Bank>
{
    public CreateBankCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<Bank> Handle(CreateBankCommand command, CancellationToken cancellationToken)
    {
        var newBank = new Bank()
        {
            Name = command.Name
        };

        DbContext.Bank.Add(newBank);
        await DbContext.SaveChangesAsync();

        return await Task.FromResult(newBank);
    }
}
