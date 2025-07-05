using Finance.Application.Repositories;
using Finance.Application.Services;
using Finance.Domain.Models;
using Finance.Domain.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application.Extensions;

public static class RepositoryExtensions
{
    public static void AddRepositories(
        this IServiceCollection services,
        IEnumerable<Type> assemblyTypes)
    {
        var repositoryTypeTuples = assemblyTypes
            .Where(t => t.Namespace == "Finance.Application.Repositories")
            .Where(t =>
            {
                var interfaces = t.GetInterfaces();
                return interfaces.Any(x => x.Name == "IRepository`2") &&
                    interfaces.All(x =>
                        x.Name != "IAppModuleRepository" &&
                        x.GetGenericArguments().All(o => o.Name != "TEntity" && o.Name != "TId"));
            })
            .Select(t => (t.GetInterfaces().First(x => x.Name == "IRepository`2"), t))
            .ToArray();

        foreach (var tuple in repositoryTypeTuples)
        {
            Type repositoryInterface = tuple.Item1;
            Type repositoryType = tuple.Item2;
            services.AddScoped(repositoryInterface, repositoryType);
        }

        services.AddScoped<IAppModuleRepository, AppModuleRepository>();
        services.AddScoped<IRepository<AppModule, Guid>, AppModuleRepository>();
        services.AddScoped<CurrencyConversionService>();
    }
    
    public static void AddEntityServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(AppModuleRepository).Assembly;
        var domainAssembly = typeof(IEntity).Assembly;
        
        var assemblyTypes = applicationAssembly.GetTypes().ToList();
        assemblyTypes.AddRange(domainAssembly.GetTypes());

        var entityServiceTypeTuples = assemblyTypes.GetEntityServiceTypes();

        foreach (var (concreteServiceInterfaceType, concreteServiceType) in entityServiceTypeTuples)
        {
            services.AddScoped(concreteServiceInterfaceType, concreteServiceType);
        }
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        var applicationAssembly = typeof(AppModuleRepository).Assembly;
        var domainAssembly = typeof(IEntity).Assembly;
        
        var assemblyTypes = applicationAssembly.GetTypes().ToList();
        assemblyTypes.AddRange(domainAssembly.GetTypes());
        
        services.AddRepositories(assemblyTypes);
    }
    
    public static IEnumerable<(Type ServiceInterfaceType, Type ServiceType)> GetEntityServiceTypes(
        this IEnumerable<Type> assemblyTypes)
    {
        // Find service interface and class types
        var servicesTypes = assemblyTypes.Where(t =>
            t.Namespace == "Finance.Application.Services" &&
            t.GetInterfaces().Any(i => i.Name.StartsWith("IEntityService")));

        var serviceInterface = servicesTypes.First(t => t.IsInterface);
        var serviceClass = servicesTypes.First(t => t.IsClass);

        // Get all repository types to match against
        var repositoryTypes = assemblyTypes
            .Where(t => t.Namespace == "Finance.Application.Repositories")
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        return assemblyTypes
            .Where(t => t.Namespace == "Finance.Domain.Models")
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(entityType =>
            {
                var idType = GetEntityIdType(entityType);
                if (idType == null)
                    return ((Type?)null, (Type?)null);

                // Check if there's a repository for this entity type
                var hasRepository = repositoryTypes.Any(repoType =>
                {
                    var repoInterfaces = repoType.GetInterfaces();
                    return repoInterfaces.Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition().Name == "IRepository`2" &&
                        i.GetGenericArguments().Length == 2 &&
                        i.GetGenericArguments()[0] == entityType &&
                        i.GetGenericArguments()[1] == idType);
                });

                if (!hasRepository)
                    return ((Type?)null, (Type?)null);

                Type serviceInterfaceType = serviceInterface.MakeGenericType(entityType, idType);
                Type serviceType = serviceClass.MakeGenericType(entityType, idType);

                return ((Type?)serviceInterfaceType, (Type?)serviceType);
            })
            .Where(tuple => tuple.Item1 != null && tuple.Item2 != null)
            .Select(tuple => (tuple.Item1!, tuple.Item2!))
            .ToArray();
    }

    private static Type? GetEntityIdType(Type entityType)
    {
        var currentType = entityType;
        while (currentType != null)
        {
            if (currentType.IsGenericType)
            {
                var genericTypeDef = currentType.GetGenericTypeDefinition();
                if (genericTypeDef.Name == "Entity`1")
                {
                    return currentType.GetGenericArguments().First();
                }
            }

            currentType = currentType.BaseType;
        }

        return null;
    }
}
