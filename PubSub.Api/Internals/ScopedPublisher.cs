using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Api
{
    internal class ScopedPublisher : IScopedPublisher
    {
        private readonly IPubSub owner;
        private readonly ConcurrentBag<(object eventData, string channel)> queue = new();
        private bool isDisposed = false;

        public ScopedPublisher(IPubSub owner)
        {
            this.owner = owner;
        }

        public Task Publish<T>(T eventData, string channel, CancellationToken token)
        {
            if (this.isDisposed)
            {
                throw new InvalidOperationException($"Cannot Publish<{typeof(T).Name}>({channel}) on disposed IScopedPublisher.");
            }

            if (!token.IsCancellationRequested)
            {
                this.queue.Add((eventData, channel));
            }
            return Task.CompletedTask;
        }

        public async Task Commit(CancellationToken token)
        {
            while (this.queue.TryTake(out var item) && !token.IsCancellationRequested)
            {
                await this.owner.Publish(item.eventData, item.channel, token).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            this.isDisposed = true;
            while (!this.queue.IsEmpty)
            {
                this.queue.TryTake(out _);
            }
        }
    }
}
