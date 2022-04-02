using System;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Api
{
    /// <summary> Publisher interface. </summary>
    public interface IPublisher
    {
        /// <summary> Published the eventData on the given channel to subscribed listeners. </summary>
        Task Publish<T>(T eventData, string channel = null, CancellationToken token = default);
    }

    /// <summary>
    /// Scoped Publisher interface to queues all Publish() Events.
    /// Events are only published when Commit() is called. Commit() can be called multiple times.
    /// If Dispose() is called (either through exiting a using statement or explicitly calling Dispose(), all queued events are cleared.
    /// After a Dispose() no more Events may be Published().
    /// </summary>
    public interface IScopedPublisher : IPublisher, IDisposable
    {
        /// <summary> Publishes all queued events. </summary>
        Task Commit(CancellationToken token = default);
    }

    /// <summary>
    /// Main Publisher/Subscriber interface.
    /// </summary>
    public interface IPubSub : IPublisher
    {
        /// <summary>
        /// Subscribe callback for all Events of the given type T on the given channel (null is channel like any other named channel).
        /// The returned ISubscriber can used to unsubscribe.
        /// </summary>
        Task<ISubscriber<T>> Subscribe<T>(Action<T> callback = null, string channel = null, CancellationToken token = default);

        /// <summary>
        /// Unsubscribe the given subscriber from receiving any more events.
        /// </summary>
        /// <returns> True if subscriber existed and was successfully removed. </returns>
        Task<bool> UnSubscribe(ISubscriber subscriber, CancellationToken token = default);

        /// <summary>
        /// Creates a scope for all Published events to be queued until:
        ///  - Commit() to publish all queued events on demand. (transaction was successful)
        ///  - Dispose() to discard all queued events. (rollback/cancel scenarios)
        /// </summary>
        IScopedPublisher CreateScope();
    }
}
