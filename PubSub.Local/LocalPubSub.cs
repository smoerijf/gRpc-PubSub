using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PubSub.Api;

namespace PubSub.Local
{
    public class LocalPubSub : IPubSub
    {
        private readonly ConcurrentDictionary<(string, Type), List<ISubscriber>> subscribers = new();

        public void Publish<T>(T eventData, string channel) where T : IEventData
        {
            if (this.subscribers.TryGetValue((channel, typeof(T)), out var subs))
            {
                List<ISubscriber> copy;
                lock (subs)
                {
                    copy = subs.ToList();
                }
                foreach (var sub in copy)
                {
                    sub.DoInvoke(eventData);
                }
            }
        }

        public ISubscriber<T> Subscribe<T>(Action<T> callback, string channel) where T : IEventData
        {
            var subs = this.subscribers.GetOrAdd((channel, typeof(T)), _ => new List<ISubscriber>());
            var sub = new Subscriber<T>(channel);
            if (callback != null)
            {
                sub.Event += d => callback(d);
            }
            lock (subs)
            {
                subs.Add(sub);
            }
            return sub;
        }

        public bool UnSubscribe(ISubscriber subscriber)
        {
            if (this.subscribers.TryGetValue((subscriber.Channel, subscriber.DataType), out var subs))
            {
                lock (subs)
                {
                    subscriber.ClearListeners();
                    return subs.Remove(subscriber);
                }
            }
            return false;
        }
    }
}
