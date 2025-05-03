using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.Banks;

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
        var bank = await bankRepository.GetByIdAsync(command.Id, cancellationToken);
        if (bank == null) throw new Exception("Bank not found");

        bank.Name = command.Name;

        await bankRepository.UpdateAsync(bank, cancellationToken);

        return await Task.FromResult(bank);
    }
}

public class UpdateBankCommand : IRequest<Bank>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}
