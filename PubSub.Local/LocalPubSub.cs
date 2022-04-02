using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PubSub.Api;

namespace PubSub.Local
{
    public class LocalPubSub : IPubSub
    {
        private readonly ConcurrentDictionary<(string channel, Type evenDataType), List<ISubscription>> subscriptions = new();

        public Task Publish<T>(T eventData, string channel, CancellationToken token) where T : IEventData
        {
            if (!token.IsCancellationRequested && this.subscriptions.TryGetValue((channel, eventData.GetType()), out var subs))
            {
                List<ISubscription> copy;
                lock (subs)
                {
                    copy = subs.ToList();
                }

                var tasks = copy.Select(sub => sub.DoInvoke(eventData, channel, token));
                return Task.WhenAll(tasks);
            }
            return Task.CompletedTask;
        }

        public Task<ISubscription<T>> Subscribe<T>(Action<string, T> callback, string channel, CancellationToken token) where T : IEventData
        {
            ISubscription<T> sub = null;
            if (!token.IsCancellationRequested)
            {
                var dataType = typeof(T);
                var subs = this.subscriptions.GetOrAdd((channel, dataType), _ => new List<ISubscription>());
                ISubscription<T> subscriber = new Subscription<T>(channel, dataType);
                if (callback != null)
                {
                    subscriber.Event += (ch, d) => callback(ch, d);
                }
                lock (subs)
                {
                    if (!token.IsCancellationRequested)
                    {
                        subs.Add(subscriber);
                        sub = subscriber;
                    }
                }
            }
            return Task.FromResult(sub);
        }

        public Task<bool> UnSubscribe(ISubscription subscriber, CancellationToken token)
        {
            if (!token.IsCancellationRequested && this.subscriptions.TryGetValue((subscriber.Channel, subscriber.DataType), out var subs))
            {
                bool removedSub;
                subscriber.ClearListeners();
                lock (subs)
                {
                    removedSub = subs.Remove(subscriber);
                }
                return Task.FromResult(removedSub);
            }
            return Task.FromResult(false);
        }

        public IScopedPublisher CreateScope()
        {
            return new ScopedPublisher(this);
        }
    }
}
