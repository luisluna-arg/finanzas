using System.ComponentModel.DataAnnotations;
using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Commands.AppModules;

public class CreateAppModuleCommand : IRequest<AppModule>
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid CurrencyId { get; set; }
}
