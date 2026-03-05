using System.Diagnostics;

namespace CQRSDispatch.Telemetry;

/// <summary>
/// Wraps <see cref="ActivitySource"/> to start a span and attach one or more
/// tags in a single call.
/// </summary>
public sealed class TaggedActivitySource
{
    private readonly ActivitySource _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaggedActivitySource"/> class.
    /// </summary>
    /// <param name="name">The name of the underlying <see cref="ActivitySource"/>.</param>
    public TaggedActivitySource(string name)
    {
        _source = new ActivitySource(name);
    }

    /// <summary>Gets the name of the underlying <see cref="ActivitySource"/>.</summary>
    public string Name => _source.Name;

    /// <summary>
    /// Starts a new activity and sets a single tag on it.
    /// Returns <see langword="null"/> when no listener is subscribed (e.g. telemetry is disabled).
    /// </summary>
    /// <param name="operationName">Name of the operation / span.</param>
    /// <param name="tagKey">Tag key.</param>
    /// <param name="tagValue">Tag value.</param>
    public Activity? StartActivity(string operationName, string tagKey, string tagValue)
    {
        var activity = _source.StartActivity(operationName);
        activity?.SetTag(tagKey, tagValue);
        return activity;
    }

    /// <summary>
    /// Starts a new activity and sets multiple tags on it.
    /// Returns <see langword="null"/> when no listener is subscribed (e.g. telemetry is disabled).
    /// </summary>
    /// <param name="operationName">Name of the operation / span.</param>
    /// <param name="tags">Key/value pairs to set as tags.</param>
    public Activity? StartActivity(string operationName, params (string Key, string Value)[] tags)
    {
        var activity = _source.StartActivity(operationName);
        if (activity is not null)
        {
            foreach (var (key, value) in tags)
            {
                activity.SetTag(key, value);
            }
        }

        return activity;
    }
}
