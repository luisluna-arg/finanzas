using FinanceApi.Application.Dtos.Factories;

namespace FinanceApi.Core.Config;

public static class ConfigExtensions
{
    public static void MainServices(this IServiceCollection services)
    {
        services.AddSingleton<IMovementDtoFactory, MovementDtoFactory>();

        services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));
    }
}
