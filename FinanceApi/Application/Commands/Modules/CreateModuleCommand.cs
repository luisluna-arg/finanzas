using System.ComponentModel.DataAnnotations;
using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Commands.Modules;

public class CreateModuleCommand : IRequest<Module>
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid CurrencyId { get; set; }
}
