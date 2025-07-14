using Microsoft.Extensions.DependencyInjection;
using CQRSDispatch.Interfaces;
using System.Reflection;

namespace CQRSDispatch.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register command and query handlers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all command and query handlers from the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection to register handlers to.</param>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    /// <param name="serviceLifetime">The service lifetime for the handlers (default is Scoped).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRequestHandlers(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        foreach (var assembly in assemblies)
        {
            RegisterCommandHandlers(services, assembly, serviceLifetime);
            RegisterQueryHandlers(services, assembly, serviceLifetime);
        }

        return services;
    }

    /// <summary>
    /// Registers command handlers from the specified assembly.
    /// </summary>
    /// <param name="services">The service collection to register handlers to.</param>
    /// <param name="assembly">The assembly to scan for command handlers.</param>
    /// <param name="serviceLifetime">The service lifetime for the handlers.</param>
    private static void RegisterCommandHandlers(IServiceCollection services, Assembly assembly, ServiceLifetime serviceLifetime)
    {
        var commandHandlerTypes = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition)
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));

        foreach (var handlerType in commandHandlerTypes)
        {
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));

            foreach (var handlerInterface in handlerInterfaces)
            {
                var genericArguments = handlerInterface.GetGenericArguments();
                var commandType = genericArguments[0];
                var resultType = genericArguments[1];

                // Validate that the command type implements ICommand
                if (typeof(ICommand).IsAssignableFrom(commandType))
                {
                    RegisterService(services, handlerInterface, handlerType, serviceLifetime);
                }
            }
        }
    }

    /// <summary>
    /// Registers query handlers from the specified assembly.
    /// </summary>
    /// <param name="services">The service collection to register handlers to.</param>
    /// <param name="assembly">The assembly to scan for query handlers.</param>
    /// <param name="serviceLifetime">The service lifetime for the handlers.</param>
    private static void RegisterQueryHandlers(IServiceCollection services, Assembly assembly, ServiceLifetime serviceLifetime)
    {
        var queryHandlerTypes = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition)
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

        foreach (var handlerType in queryHandlerTypes)
        {
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

            foreach (var handlerInterface in handlerInterfaces)
            {
                var genericArguments = handlerInterface.GetGenericArguments();
                var queryType = genericArguments[0];
                var resultType = genericArguments[1];

                // Validate that the query type implements IQuery<TResult>
                var queryInterfaceType = typeof(IQuery<>).MakeGenericType(resultType);
                if (queryInterfaceType.IsAssignableFrom(queryType))
                {
                    RegisterService(services, handlerInterface, handlerType, serviceLifetime);
                }
            }
        }
    }

    /// <summary>
    /// Registers a service with the specified service lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type (interface).</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="serviceLifetime">The service lifetime.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown service lifetime is provided.</exception>
    private static void RegisterService(IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton(serviceType, implementationType);
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped(serviceType, implementationType);
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(serviceType, implementationType);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
        }
    }
}
