using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.CreditCards;

public class CreateCreditCardIssuerCommandHandler : BaseResponseHandler<CreateCreditCardIssuerCommand, CreditCardIssuer>
{
    private readonly IRepository<CreditCardIssuer, Guid> creditCardIssuerRepository;
    private readonly IRepository<Bank, Guid> bankRepository;

    public CreateCreditCardIssuerCommandHandler(
        FinanceDbContext db,
        IRepository<Bank, Guid> bankRepository,
        IRepository<CreditCardIssuer, Guid> creditCardIssuerRepository)
        : base(db)
    {
        this.bankRepository = bankRepository;
        this.creditCardIssuerRepository = creditCardIssuerRepository;
    }

    public override async Task<CreditCardIssuer> Handle(CreateCreditCardIssuerCommand command, CancellationToken cancellationToken)
    {
        var bank = await bankRepository.GetById(command.BankId);
        if (bank == null) throw new Exception("Bank not found");

        var newCreditCardIssuer = new CreditCardIssuer()
        {
            Bank = bank,
            Name = command.Name
        };

        await creditCardIssuerRepository.Add(newCreditCardIssuer);

        return await Task.FromResult(newCreditCardIssuer);
    }
}

public class CreateCreditCardIssuerCommand : IRequest<CreditCardIssuer>
{
    [Required]
    public Guid BankId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
}
