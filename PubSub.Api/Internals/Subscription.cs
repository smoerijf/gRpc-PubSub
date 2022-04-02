using System;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Api
{
    internal class Subscription<T> : ISubscription<T>
    {
        public event ISubscription<T>.EventData Event;

        public Type DataType { get; }
        public string Channel { get; }

        public Subscription(string channel, Type dataType)
        {
            this.DataType = dataType;
            this.Channel = channel;
        }

        public Task DoInvoke(object eventData, string channel, CancellationToken token) => this.DoInvoke((T)eventData, channel, token);

        public Task DoInvoke(T eventData, string channel, CancellationToken token) => Task.Run(() => this.Event?.Invoke(channel, eventData), token);

        public void ClearListeners() => this.Event = null;
    }
}
