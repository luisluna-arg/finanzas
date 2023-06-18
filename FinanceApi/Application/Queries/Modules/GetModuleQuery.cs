using FinanceApi.Application.Models;
using MediatR;

namespace FinanceApi.Application.Queries.Modules;

public class GetModuleQuery : IRequest<Module>
{
    public Guid Id { get; set; }
}
