using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf.Grpc;

namespace PubSub.Grpc
{
    public class PubSubGrpcService : IPubSubGrpcService
    {
        private readonly IServerSubscriptionFactory serverSubscriptionFactory;

        public PubSubGrpcService(IServerSubscriptionFactory serverSubscriptionFactory)
        {
            this.serverSubscriptionFactory = serverSubscriptionFactory;
        }
        
        public async IAsyncEnumerable<EventData> Subscribe(Guid clientId, IAsyncEnumerable<SubscriptionData> subscriptionStream, CallContext context)
        {
            var client = this.serverSubscriptionFactory.GetSubscriptionsForClient(clientId);
            _ = this.HandleSubscriptionRequests(client, subscriptionStream, context.CancellationToken);

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var eventData = await client.GetNextEventDataAsync(context.CancellationToken);
                if (eventData != null)
                {
                    yield return eventData;
                }
            }
        }
        
        private Task HandleSubscriptionRequests(IClientSubscriptions client, IAsyncEnumerable<SubscriptionData> subscriptionStream, CancellationToken token) => Task.Run(async () =>
        {
            await foreach (var subscription in subscriptionStream.WithCancellation(token))
            {
                if (subscription.Action == SubscriptionData.Actions.Add)
                {
                    client.AddSubscription(subscription.Channel, subscription.DataType);
                }
                else
                {
                    client.RemoveSubscription(subscription.Channel, subscription.DataType);
                }
            }
        }, token);
    }
}
