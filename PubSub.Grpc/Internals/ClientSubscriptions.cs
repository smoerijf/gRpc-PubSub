using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using PubSub.Api;

namespace PubSub.Grpc
{
    public interface IClientSubscriptions
    {
        void AddSubscription(string channel, Type dataType);

        void RemoveSubscription(string channel, Type dataType);

        Task<EventData> GetNextEventDataAsync(CancellationToken token);
    }

    internal sealed class ClientSubscriptions : IClientSubscriptions
    {
        private readonly ConcurrentDictionary<(string channel, Type), object> subscriptions = new();
        private readonly BufferBlock<EventData> eventPublishQueue = new();

        public void AddSubscription(string channel, Type dataType)
        {
            this.subscriptions.TryAdd((channel, dataType), null);
        }

        public void RemoveSubscription(string channel, Type dataType)
        {
            this.subscriptions.TryRemove((channel, dataType), out _);
        }

        public async Task<EventData> GetNextEventDataAsync(CancellationToken token)
        {
            try
            {
                return await this.eventPublishQueue.ReceiveAsync(token);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }
        
        public void Publish<T>(T eventData, string channel) where T : IEventData
        {
            // If any subscription exists for the channel/eventData: queue it to be published to that client
            if (this.subscriptions.ContainsKey((channel, eventData.GetType())))
            {
                this.eventPublishQueue.Post(new EventData(channel, eventData));
            }
        }
    }
}
