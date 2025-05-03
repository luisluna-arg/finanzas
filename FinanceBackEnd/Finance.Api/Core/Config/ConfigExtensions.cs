using Castle.DynamicProxy.Internal;
using Finance.Application.Repositories;
using Finance.Application.Services;
using Finance.Domain.DataConverters;
using Finance.Domain.Models;
using Finance.Domain.Models.Interfaces;

namespace Finance.Api.Core.Config;

public static class ConfigExtensions
{
    public static void MainServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(AppModuleRepository).Assembly;
        var domainAssembly = typeof(IEntity).Assembly;

        services.AddAutoMapper(applicationAssembly);

        services.AddMediatR(o => o.RegisterServicesFromAssembly(applicationAssembly));

        var assemblyTypes = applicationAssembly.GetTypes().ToList();
        assemblyTypes.AddRange(domainAssembly.GetTypes());

        services.AddRepositories(assemblyTypes);

        services.AddEntityServices(assemblyTypes);

        services.AddScoped<ICurrencyConverter, CurrencyConverter>();
    }

    private static void AddRepositories(
        this IServiceCollection services,
        IEnumerable<Type> assemblyTypes)
    {
        var repositoryTypeTuples = assemblyTypes
            .Where(t => t.Namespace == "Finance.Application.Repositories")
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
        services.AddScoped<CurrencyConversionService>();
    }

    private static void AddEntityServices(
        this IServiceCollection services,
        IEnumerable<Type> assemblyTypes)
    {
        var servicesTypes = assemblyTypes.Where(t =>
            t.Namespace == "Finance.Application.Services" &&
            t.GetAllInterfaces().Any(i => i.Name.StartsWith("IEntityService")));

        var serviceInterface = servicesTypes.First(t => t.IsInterface);

        var serviceClass = servicesTypes.First(t => t.IsClass);

        var entityServiceTypeTuples = assemblyTypes
            .Where(t => t.Namespace == "Finance.Domain.Models")
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
