using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace StarDust.CasparCG.Events;

/// <summary>
/// Provides filtered asynchronous access to CasparCG domain events.
/// </summary>
public sealed class CasparEventStream
{
    private readonly ChannelReader<CasparEvent> _reader;
    private readonly Func<CasparEvent, bool> _predicate;

    /// <summary>
    /// Initializes a new event stream wrapper.
    /// </summary>
    /// <param name="reader">The underlying event reader.</param>
    /// <param name="predicate">The optional filter predicate.</param>
    public CasparEventStream(ChannelReader<CasparEvent> reader, Func<CasparEvent, bool>? predicate = null)
    {
        _reader = reader;
        _predicate = predicate ?? (_ => true);
    }

    /// <summary>
    /// Narrows the stream to a specific channel.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <returns>A filtered event stream.</returns>
    public CasparEventStream ForChannel(int channel) => new(_reader, evt => _predicate(evt) && evt.Channel == channel);

    /// <summary>
    /// Narrows the stream to a specific layer.
    /// </summary>
    /// <param name="layer">The target layer.</param>
    /// <returns>A filtered event stream.</returns>
    public CasparEventStream ForLayer(int layer) => new(_reader, evt => _predicate(evt) && evt.Layer == layer);

    /// <summary>
    /// Narrows the stream to a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The event type to include.</typeparam>
    /// <returns>A typed filtered event stream.</returns>
    public TypedCasparEventStream<TEvent> OfType<TEvent>() where TEvent : CasparEvent =>
        new(_reader, evt => _predicate(evt) && evt is TEvent);

    /// <summary>
    /// Reads all matching events from the underlying channel.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous sequence of matching events.</returns>
    public async IAsyncEnumerable<CasparEvent> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var evt in _reader.ReadAllAsync(cancellationToken))
        {
            if (_predicate(evt))
            {
                yield return evt;
            }
        }
    }
}

/// <summary>
/// Provides typed asynchronous access to CasparCG domain events.
/// </summary>
/// <typeparam name="TEvent">The event type to read.</typeparam>
public sealed class TypedCasparEventStream<TEvent> where TEvent : CasparEvent
{
    private readonly ChannelReader<CasparEvent> _reader;
    private readonly Func<CasparEvent, bool> _predicate;

    /// <summary>
    /// Initializes a new typed event stream wrapper.
    /// </summary>
    /// <param name="reader">The underlying event reader.</param>
    /// <param name="predicate">The event predicate.</param>
    public TypedCasparEventStream(ChannelReader<CasparEvent> reader, Func<CasparEvent, bool> predicate)
    {
        _reader = reader;
        _predicate = predicate;
    }

    /// <summary>
    /// Reads all matching typed events from the underlying channel.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous sequence of matching typed events.</returns>
    public async IAsyncEnumerable<TEvent> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var evt in _reader.ReadAllAsync(cancellationToken))
        {
            if (_predicate(evt))
            {
                yield return (TEvent)evt;
            }
        }
    }
}
