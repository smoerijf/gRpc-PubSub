using System;

namespace PubSub.Api
{
    public interface IPubSub
    {
        void Publish<T>(T eventData, string channel = null) where T : IEventData;
        ISubscriber<T> Subscribe<T>(Action<T> callback = null, string channel = null) where T : IEventData;
        bool UnSubscribe(ISubscriber callback);
    }

    public interface ISubscriber
    {
        Type DataType { get; }
        string Channel { get; }

        void DoInvoke(object eventData);
        void ClearListeners();
    }

    public delegate void EventData<in T>(T eventData);

    public interface ISubscriber<T> : ISubscriber where T : IEventData
    {
        event EventData<T> Event;
        void DoInvoke(T eventData);
    }

    internal class Subscriber<T> : ISubscriber<T> where T : IEventData
    {
        public event EventData<T> Event;

        public Type DataType { get; }
        public string Channel { get; }

        public Subscriber(string channel)
        {
            this.DataType = typeof(T);
            this.Channel = channel;
        }
        public void DoInvoke(object eventData) => this.DoInvoke((T)eventData);
        public void DoInvoke(T eventData) => this.Event?.Invoke(eventData);
        public void ClearListeners() => this.Event = null;
    }

    public interface IEventData
    {
    }
}
