using System.ComponentModel.DataAnnotations;
using MediatR;

namespace FinanceApi.Application.Commands.Currencies;

public class DeleteCurrencyCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }
}
