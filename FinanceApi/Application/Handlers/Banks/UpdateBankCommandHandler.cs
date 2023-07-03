using FinanceApi.Application.Commands.Banks;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Banks;

public class UpdateBankCommandHandler : BaseResponseHandler<UpdateBankCommand, Bank>
{
    public UpdateBankCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<Bank> Handle(UpdateBankCommand command, CancellationToken cancellationToken)
    {
        var bank = await GetBank(command.Id);

        bank.Name = command.Name;

        await DbContext.SaveChangesAsync();

        return await Task.FromResult(bank);
    }

    private async Task<Bank> GetBank(Guid id)
    {
        var bank = await DbContext.Bank.FirstOrDefaultAsync(o => o.Id == id);

        if (bank == null) throw new Exception("Bank not found");

        return bank;
    }
}
