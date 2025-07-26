using Finance.Application.Commands.FundOwners;
using Finance.Application.Services;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Requests.Funds;
using Finance.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application.Extensions;

public static class SagaServiceExtensions
{
    public static void AddSagaServices(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        services.AddScoped<IdentityService>();
        services.AddScoped<FundResourceOwnerService>();
        services.AddScoped<FundService>();
        services.AddScoped<ISagaService<CreateFundSagaRequest, UpdateFundSagaRequest, DeleteFundSagaRequest, Fund>, FundService>();
        services.AddScoped<IResourceOwnerSagaService<SetFundOwnerSagaRequest, DeleteFundOwnerSagaRequest, FundResource>, FundResourceOwnerService>();
    }
}