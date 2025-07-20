using Finance.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application.Extensions;

public static class SagaServiceExtensions
{
    public static void AddSagaServices(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        services.AddScoped<IdentityService>();
        services.AddScoped<FundResourceOwnerService>();
    }
}