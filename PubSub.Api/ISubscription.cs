using System;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Api
{
    /// <summary> Subscription identification interface. </summary>
    public interface ISubscription
    {
        Type DataType { get; }
        string Channel { get; }

        /// <summary> Publish the eventData to all subscribed listeners. </summary>
        Task DoInvoke(object eventData, string channel, CancellationToken token = default);

        /// <summary> Removes all listeners on this subscriber. </summary>
        void ClearListeners();
    }
    
    public interface ISubscription<T> : ISubscription
    {
        public delegate void EventData(string channel, T eventData);

        /// <summary> The event that will be invoked when eventData needs to published to all listeners. </summary>
        event EventData Event;

        /// <summary> Publish the eventData to all subscribed listeners. </summary>
        Task DoInvoke(T eventData, string channel, CancellationToken token = default);
    }
}
