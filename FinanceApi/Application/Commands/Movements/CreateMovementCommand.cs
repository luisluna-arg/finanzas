using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Commands.Movements;

public class CreateMovementCommand : CreateMovementBaseCommand
{
    public Guid? AppModuleId { get; set; }
}
