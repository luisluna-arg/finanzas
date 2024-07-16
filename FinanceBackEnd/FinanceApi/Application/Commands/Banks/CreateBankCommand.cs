using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.Banks;

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

        await bankRepository.AddAsync(newBank, cancellationToken);

        return await Task.FromResult(newBank);
    }
}

public class CreateBankCommand : IRequest<Bank>
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
