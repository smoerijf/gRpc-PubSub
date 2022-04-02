using System;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Api
{
    public interface ISubscriber
    {
        Type DataType { get; }
        string Channel { get; }

        Task DoInvoke(object eventData, CancellationToken token = default);
        void ClearListeners();
    }

    public delegate void EventData<in T>(T eventData);

    public interface ISubscriber<T> : ISubscriber where T : IEventData
    {
        event EventData<T> Event;

        Task DoInvoke(T eventData, CancellationToken token = default);
    }

    internal class Subscriber<T> : ISubscriber<T> where T : IEventData
    {
        public event EventData<T> Event;

        public Type DataType { get; }
        public string Channel { get; }

        public Subscriber(string channel)
        {
            this.DataType = typeof(T);
            this.Channel = channel;
        }

        public Task DoInvoke(object eventData, CancellationToken token) => this.DoInvoke((T)eventData, token);

        public Task DoInvoke(T eventData, CancellationToken token) => Task.Run(() => this.Event?.Invoke(eventData), token);

        public void ClearListeners() => this.Event = null;
    }
}
