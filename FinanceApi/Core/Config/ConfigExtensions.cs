using Castle.DynamicProxy.Internal;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Core.Config;

public static class ConfigExtensions
{
    public static void MainServices(this IServiceCollection services)
    {
        services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));

        services.AddAutoMapper(typeof(Program));

        var assembly = typeof(Program).Assembly;
        var assemblyTypes = assembly.GetTypes();

        services.AddRepositories(assemblyTypes);

        services.AddEntityServices(assemblyTypes);
    }

    private static void AddRepositories(
        this IServiceCollection services,
        IEnumerable<Type> assemblyTypes
    )
    {
        var repositoryTypeTuples = assemblyTypes
            .Where(t => t.Namespace == "FinanceApi.Infrastructure.Repositories")
            .Where(t =>
            {
                var interfaces = t.GetAllInterfaces();
                return interfaces.Any(x => x.Name == "IRepository`2") &&
                    interfaces.All(x =>
                        x.Name != "IAppModuleRepository" &&
                        x.GetGenericArguments().All(o => o.Name != "TEntity" && o.Name != "TId"));
            })
            .Select(t => (t.GetAllInterfaces().First(x => x.Name == "IRepository`2"), t))
            .ToArray();

        foreach (var (repositoryInterface, repositoryType) in repositoryTypeTuples)
        {
            services.AddScoped(repositoryInterface, repositoryType);
        }

        services.AddScoped<IAppModuleRepository, AppModuleRepository>();
        services.AddScoped<IRepository<AppModule, Guid>, AppModuleRepository>();
    }

    private static void AddEntityServices(
        this IServiceCollection services,
        IEnumerable<Type> assemblyTypes
    )
    {
        var servicesTypes = assemblyTypes.Where(t =>
            t.Namespace == "FinanceApi.Infrastructure.Services");

        var serviceInterface = servicesTypes.First(t => t.IsInterface);

        var serviceClass = servicesTypes.First(t => t.IsClass);

        var entityServiceTypeTuples = assemblyTypes
            .Where(t => t.Namespace == "FinanceApi.Domain.Models")
            .Select(entityType =>
            {
                var idType = entityType.BaseType!.GetGenericArguments().First();

                Type serviceInterfaceType = serviceInterface.MakeGenericType(entityType, idType);
                Type serviceType = serviceClass.MakeGenericType(entityType, idType);

                return (serviceInterfaceType, serviceType);
            })
            .ToArray();

        foreach (var (concreteServiceInterfaceType, concreteServiceType) in entityServiceTypeTuples)
        {
            services.AddScoped(concreteServiceInterfaceType, concreteServiceType);
        }
    }
}
