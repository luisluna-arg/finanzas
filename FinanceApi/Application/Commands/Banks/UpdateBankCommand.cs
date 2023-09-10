using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;
using MediatR;

namespace FinanceApi.Application.Commands.Banks;

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
        if (bank == null) throw new Exception("Bank not found");

        bank.Name = command.Name;

        await bankRepository.Update(bank);

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
