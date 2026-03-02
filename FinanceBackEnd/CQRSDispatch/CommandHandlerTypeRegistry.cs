using System.Collections.Concurrent;

namespace CQRSDispatch;

/// <summary>
/// Maps command types to their registered handler interface types.
/// Built once at startup during <see cref="Extensions.ServiceCollectionExtensions.AddRequestHandlers"/>
/// and consumed by the dispatcher to resolve handlers without runtime assembly scanning.
/// </summary>
public sealed class CommandHandlerTypeRegistry
{
    private readonly ConcurrentDictionary<Type, List<Type>> _map = new();

    /// <summary>
    /// Records that <paramref name="handlerInterfaceType"/> handles commands of type <paramref name="commandType"/>.
    /// </summary>
    public void Register(Type commandType, Type handlerInterfaceType)
    {
        _map.AddOrUpdate(
            commandType,
            _ => [handlerInterfaceType],
            (_, list) => { list.Add(handlerInterfaceType); return list; });
    }

    /// <summary>
    /// Returns all handler interface types registered for the given command type, or null if none.
    /// </summary>
    public IReadOnlyList<Type>? GetHandlerInterfaceTypes(Type commandType)
    {
        return _map.TryGetValue(commandType, out var list) ? list : null;
    }
}
