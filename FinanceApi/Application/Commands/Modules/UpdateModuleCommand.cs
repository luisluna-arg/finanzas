using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Models;
using MediatR;

namespace FinanceApi.Application.Commands.Modules;

public class UpdateModuleCommand : IRequest<Module>
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid CurrencyId { get; set; }
}
