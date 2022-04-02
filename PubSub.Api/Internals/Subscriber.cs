using System;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Api
{
    internal class Subscriber<T> : ISubscriber<T>
    {
        public event ISubscriber<T>.EventData Event;

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
