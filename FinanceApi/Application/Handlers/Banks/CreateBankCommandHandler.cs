using FinanceApi.Application.Commands.Banks;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Banks;

public class CreateBankCommandHandler : BaseResponseHandler<CreateBankCommand, Bank>
{
    private readonly IRepository<Bank, Guid> bankRepository;

    public CreateBankCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
    }

    public override async Task<Bank> Handle(CreateBankCommand command, CancellationToken cancellationToken)
    {
        var newBank = new Bank()
        {
            Name = command.Name
        };

        await bankRepository.Add(newBank);

        return await Task.FromResult(newBank);
    }
}
