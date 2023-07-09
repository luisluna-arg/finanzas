using FinanceApi.Application.Commands.Banks;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Banks;

public class UpdateBankCommandHandler : BaseResponseHandler<UpdateBankCommand, Bank>
{
    private readonly IRepository<Bank, Guid> bankRepository;

    public UpdateBankCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
    }

    public override async Task<Bank> Handle(UpdateBankCommand command, CancellationToken cancellationToken)
    {
        var bank = await bankRepository.GetById(command.Id);

        bank.Name = command.Name;

        await bankRepository.Update(bank);

        return await Task.FromResult(bank);
    }
}
