using FinanceApi.DtoFactory;
using FinanceApi.Services;

namespace FinanceApi.Core.Config;

public static class ConfigExtensions
{
    public static void MainServices(this IServiceCollection services)
    {
        services.AddSingleton<IMovementDtoFactory, MovementDtoFactory>();

        services.AddScoped<IMovementsService, MovementsService>();

    }
}