using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Models;
using MediatR;

namespace FinanceApi.Application.Commands.Currencies;

public class CreateCurrencyCommand : IRequest<Currency>
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string ShortName { get; set; } = string.Empty;
}
