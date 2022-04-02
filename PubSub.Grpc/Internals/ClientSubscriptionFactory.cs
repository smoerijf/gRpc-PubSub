using System;
using System.Threading;
using System.Threading.Tasks;
using PubSub.Api;
using PubSub.Local;

namespace PubSub.Grpc
{
    public interface IClientSubscriptionFactory
    {
        Task<bool> UnSubscribe(ISubscription subscription, CancellationToken token = default);

        Task<ISubscription<T>> Subscribe<T>(Action<string, T> callback, string channel, CancellationToken token = default) where T : IEventData;
    }
    
    internal sealed class ClientSubscriptionFactory : IClientSubscriptionFactory
    {
        private readonly IPubSub local = new LocalPubSub();
        private readonly Func<IClientCommunicator> clientCommunicatorFactory;
        private IClientCommunicator clientCommunicator;

        private IClientCommunicator ClientCommunicator => this.clientCommunicator ??= this.clientCommunicatorFactory().Init(this.local.Publish);

        public ClientSubscriptionFactory(Func<IClientCommunicator> clientCommunicatorFactory)
        {
            this.clientCommunicatorFactory = clientCommunicatorFactory;
        }

        public async Task<ISubscription<T>> Subscribe<T>(Action<string, T> callback, string channel, CancellationToken token) where T : IEventData
        {
            await this.ClientCommunicator.ExecuteSubscriptionCommand(new SubscriptionData(channel, typeof(T), SubscriptionData.Actions.Add));
            return await this.local.Subscribe(callback, channel, token);
        }

        public async Task<bool> UnSubscribe(ISubscription subscription, CancellationToken token)
        {
            await this.ClientCommunicator.ExecuteSubscriptionCommand(new SubscriptionData(subscription.Channel, subscription.DataType, SubscriptionData.Actions.Remove));
            return await this.local.UnSubscribe(subscription, token);
        }
    }
}
