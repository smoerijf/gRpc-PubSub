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
        private readonly ConcurrentDictionary<(string channel, Type evenDataType), List<ISubscriber>> subscribers = new();

        public Task Publish<T>(T eventData, string channel, CancellationToken token)
        {
            if (!token.IsCancellationRequested && this.subscribers.TryGetValue((channel, eventData.GetType()), out var subs))
            {
                List<ISubscriber> copy;
                lock (subs)
                {
                    copy = subs.ToList();
                }

                var tasks = copy.Select(sub => sub.DoInvoke(eventData, token));
                return Task.WhenAll(tasks);
            }
            return Task.CompletedTask;
        }

        public Task<ISubscriber<T>> Subscribe<T>(Action<T> callback, string channel, CancellationToken token)
        {
            ISubscriber<T> sub = null;
            if (!token.IsCancellationRequested)
            {
                var subs = this.subscribers.GetOrAdd((channel, typeof(T)), _ => new List<ISubscriber>());
                ISubscriber<T> subscriber = new Subscriber<T>(channel);
                if (callback != null)
                {
                    subscriber.Event += d => callback(d);
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

        public Task<bool> UnSubscribe(ISubscriber subscriber, CancellationToken token)
        {
            if (!token.IsCancellationRequested && this.subscribers.TryGetValue((subscriber.Channel, subscriber.DataType), out var subs))
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
