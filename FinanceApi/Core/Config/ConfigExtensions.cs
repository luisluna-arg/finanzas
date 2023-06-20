using FinanceApi.Application.Dtos.Factories;
using FinanceApi.Services;

namespace FinanceApi.Core.Config;

public static class ConfigExtensions
{
    public static void MainServices(this IServiceCollection services)
    {
        services.AddSingleton<IMovementDtoFactory, MovementDtoFactory>();

        services.AddScoped<IMovementsService, MovementsService>();

        services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));
    }
}
