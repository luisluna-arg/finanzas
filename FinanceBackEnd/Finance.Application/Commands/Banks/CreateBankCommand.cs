using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;

namespace Finance.Application.Commands.Banks;

public class CreateBankCommandHandler : BaseCommandHandler<CreateBankCommand, Bank>
{
    private readonly IRepository<Bank, Guid> bankRepository;

    public CreateBankCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
    }

    public override async Task<DataResult<Bank>> ExecuteAsync(CreateBankCommand command, CancellationToken cancellationToken)
    {
        var newBank = new Bank()
        {
            Name = command.Name
        };

        await bankRepository.AddAsync(newBank, cancellationToken);

        return DataResult<Bank>.Success(newBank);
    }
}

public class CreateBankCommand : ICommand
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
