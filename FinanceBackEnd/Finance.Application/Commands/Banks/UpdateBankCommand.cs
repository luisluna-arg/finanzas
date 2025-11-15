using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.Banks;
using Finance.Persistence;

namespace Finance.Application.Commands.Banks;

public class UpdateBankCommandHandler : BaseCommandHandler<UpdateBankCommand, Bank>
{
    private readonly IRepository<Bank, Guid> bankRepository;

    public UpdateBankCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
    }

    public override async Task<DataResult<Bank>> ExecuteAsync(UpdateBankCommand command, CancellationToken cancellationToken)
    {
        var bank = await bankRepository.GetByIdAsync(command.Id, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        bank.Name = command.Name;

        await bankRepository.UpdateAsync(bank, cancellationToken);

        return DataResult<Bank>.Success(bank);
    }
}

public class UpdateBankCommand : ICommand
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
}
