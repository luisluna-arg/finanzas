using Finance.Application.Repositories;
using Finance.Application.Services;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application.Extensions;

public static class RepositoryExtensions
{
    public static void AddRepositories(
        this IServiceCollection services,
        IEnumerable<Type> assemblyTypes)
    {
        // Find all repository types except AppModuleRepository (it's registered separately)
        var repositoryTypeTuples = assemblyTypes
            .Where(t => t.Namespace == "Finance.Application.Repositories")
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.Name != "AppModuleRepository")
            .Where(t =>
            {
                var interfaces = t.GetInterfaces();
                return interfaces.Any(x => x.Name == "IRepository`2");
            })
            .Select(t => (t.GetInterfaces().First(x => x.Name == "IRepository`2"), t))
            .ToArray();

        foreach (var tuple in repositoryTypeTuples)
        {
            Type repositoryInterface = tuple.Item1;
            Type repositoryType = tuple.Item2;
            services.AddScoped(repositoryInterface, repositoryType);
        }

        // First register the specific repository
        services.AddScoped<IAppModuleRepository, AppModuleRepository>();

        // Then register the generic repository with the concrete implementation
        // This ensures that when IRepository<AppModule, Guid> is requested, 
        // an AppModuleRepository instance is provided, not an IAppModuleRepository
        services.AddScoped<IRepository<AppModule, Guid>, AppModuleRepository>();

        // Register entity service for AppModule manually
        services.AddScoped<IEntityService<AppModule, Guid>, EntityService<AppModule, Guid>>();

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
        // Find the IEntityService<,> interface and EntityService<,> class types directly
        var serviceInterface = typeof(IEntityService<,>);
        var serviceClass = typeof(EntityService<,>);

        if (serviceInterface == null || serviceClass == null)
        {
            return Enumerable.Empty<(Type, Type)>();
        }

        // Get all repository types to match against - only concrete implementations
        var repositoryTypes = assemblyTypes
            .Where(t => t.Namespace == "Finance.Application.Repositories")
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface)
            .ToList();

        // Get repository interfaces to exclude special cases
        var repositoryInterfaces = assemblyTypes
            .Where(t => (t.Namespace == "Finance.Application.Repositories" || t.Namespace == "Finance.Application.Repositories.Base") && t.IsInterface)
            .ToList();

        // Special case for IAppModuleRepository - skip it as it's registered manually
        var specialRepositoryInterfaces = repositoryInterfaces
            .Where(t => t.Name == "IAppModuleRepository")
            .ToList();

        return assemblyTypes
            .Where(t => t.Namespace == "Finance.Domain.Models")
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(entityType =>
            {
                var idType = GetEntityIdType(entityType);
                if (idType == null)
                    return ((Type?)null, (Type?)null);

                // Skip AppModule as it has a special repository registration
                if (entityType.Name == "AppModule")
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
