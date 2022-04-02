using System;
using System.Collections.Concurrent;
using PubSub.Api;

namespace PubSub.Grpc
{
    public interface IServerSubscriptionFactory
    {
        IClientSubscriptions GetSubscriptionsForClient(Guid clientId);

        void Publish<T>(T eventData, string channel) where T : IEventData;
    }

    internal sealed class ServerSubscriptionFactory : IServerSubscriptionFactory
    {
        private readonly ConcurrentDictionary<Guid, ClientSubscriptions> clients = new();

        public IClientSubscriptions GetSubscriptionsForClient(Guid clientId)
        {
            return this.clients.GetOrAdd(clientId, _ => new ClientSubscriptions());
        }

        public void Publish<T>(T eventData, string channel) where T : IEventData
        {
            // Find all clients that are subscribed to channel/eventDataType combination
            foreach (var client in this.clients)
            {
                client.Value.Publish(eventData, channel);
            }
        }
    }
}
