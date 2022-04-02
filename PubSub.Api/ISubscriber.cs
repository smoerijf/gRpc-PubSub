using System;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Api
{
    /// <summary> Subscriber identification interface. </summary>
    public interface ISubscriber
    {
        Type DataType { get; }
        string Channel { get; }

        /// <summary> Publish the eventData to all subscribed listeners. </summary>
        Task DoInvoke(object eventData, CancellationToken token = default);

        /// <summary> Removes all listeners on this subscriber. </summary>
        void ClearListeners();
    }
    
    public interface ISubscriber<T> : ISubscriber
    {
        public delegate void EventData(T eventData);

        /// <summary> The event that will be invoked when eventData needs to published to all listeners. </summary>
        event EventData Event;

        /// <summary> Publish the eventData to all subscribed listeners. </summary>
        Task DoInvoke(T eventData, CancellationToken token = default);
    }
}
