using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PubSub.Api;
using PubSub.Local;

namespace PubSub.Grpc
{
    public class GrpcPubSub : IPubSub
    {
        private readonly LocalPubSub local = new LocalPubSub();
        private readonly IServerSubscriptionFactory serverSubscriptionFactory;
        private readonly IClientSubscriptionFactory clientSubscriptionFactory;

        public GrpcPubSub(IServerSubscriptionFactory serverSubscriptionFactory, IClientSubscriptionFactory clientSubscriptionFactory)
        {
            this.serverSubscriptionFactory = serverSubscriptionFactory;
            this.clientSubscriptionFactory = clientSubscriptionFactory;
        }

        public Task Publish<T>(T eventData, string channel, CancellationToken token) where T : IEventData
        {
            // TODO: Publish to local subscriptions on the server
            this.serverSubscriptionFactory.Publish(eventData, channel);
            return Task.CompletedTask;
        }

        public Task<ISubscription<T>> Subscribe<T>(Action<string, T> callback, string channel, CancellationToken token) where T : IEventData
        {
            // TODO: Subscribe locally
            // First subscribe should open the gRPC duplex streaming call
            // Which gRPC server to use?
            return this.clientSubscriptionFactory.Subscribe(callback, channel, token);
        }

        public Task<bool> UnSubscribe(ISubscription subscriber, CancellationToken token)
        {
            // TODO: Unsubscribe locally
            return this.clientSubscriptionFactory.UnSubscribe(subscriber, token);
        }

        public IScopedPublisher CreateScope()
        {
            return new ScopedPublisher(this);
        }
    }
}
