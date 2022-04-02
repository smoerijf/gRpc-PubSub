using System;
using System.Threading;
using System.Threading.Tasks;
using PubSub.Api;

namespace PubSub.Grpc
{
    internal sealed class GrpcSubscription : ISubscription
    {
        public event ISubscription<object>.EventData Event;

        public Type DataType { get; }
        public string Channel { get; }

        public GrpcSubscription(string channel, Type eventDataType)
        {
            this.DataType = eventDataType;
            this.Channel = channel;
        }

        public Task DoInvoke(object eventData, string channel, CancellationToken token) => Task.Run(() => this.Event?.Invoke(channel, eventData), token);

        public void ClearListeners() => this.Event = null;
    }
}
