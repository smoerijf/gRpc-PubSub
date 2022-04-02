using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using PubSub.Api;

namespace PubSub.Grpc
{ 
    public interface IClientCommunicator
    {
        Guid ClientId { get; }

        Task ExecuteSubscriptionCommand(SubscriptionData subscription);

        IClientCommunicator Init(Func<IEventData, string, CancellationToken, Task> publishCallback);

        Task Stop();
    }

    internal sealed class ClientCommunicator : IClientCommunicator
    {
        private readonly IPubSubGrpcService pubSubService;
        private readonly BufferBlock<SubscriptionData> subscriptionQueue = new();
        private readonly CancellationTokenSource tokenSource = new();
        private Func<IEventData, string, CancellationToken, Task> publishCallback;
        private Task subscriptionRequestTask;

        public Guid ClientId { get; } = Guid.NewGuid();

        public ClientCommunicator(IPubSubGrpcService pubSubService)
        {
            this.pubSubService = pubSubService;
        }

        private async IAsyncEnumerable<SubscriptionData> CreateSubscriptionStream([EnumeratorCancellation] CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var subscription = await this.GetNextSubscriptionAsync(token);
                if (subscription != null)
                {
                    yield return subscription;
                }
            }
        }

        public async Task<SubscriptionData> GetNextSubscriptionAsync(CancellationToken token)
        {
            try
            {
                return await this.subscriptionQueue.ReceiveAsync(token);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        private async Task InitiateSubscriptionRequest(CancellationToken token)
        {
            var subscriptionStream = this.CreateSubscriptionStream(token);
            await foreach (var eventData in this.pubSubService.Subscribe(this.ClientId, subscriptionStream).WithCancellation(token))
            {
                await this.publishCallback(eventData.Data, eventData.Channel, token);
            }
        }

        public Task ExecuteSubscriptionCommand(SubscriptionData subscription)
        {
            this.subscriptionQueue.Post(subscription);
            return Task.CompletedTask;
        }

        public IClientCommunicator Init(Func<IEventData, string, CancellationToken, Task> publishCallback)
        {
            this.publishCallback = publishCallback;
            this.subscriptionRequestTask = this.InitiateSubscriptionRequest(this.tokenSource.Token);
            return this;
        }

        public Task Stop()
        {
            this.tokenSource.Cancel();
            return this.subscriptionRequestTask;
        }
    }
}
