using FinanceApi.Application.Models;
using MediatR;

namespace FinanceApi.Application.Queries.Modules;

public class GetAllModulesQuery : IRequest<Module[]>
{
}
