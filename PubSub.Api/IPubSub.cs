using System;
using System.Threading;
using System.Threading.Tasks;

namespace PubSub.Api
{
    public interface IPublisher
    {
        Task Publish<T>(T eventData, string channel = null, CancellationToken token = default) where T : IEventData;
    }

    public interface IScopedPublisher : IPublisher, IDisposable
    {
        Task Commit(CancellationToken token = default);
    }

    public interface IPubSub : IPublisher
    {
        Task<ISubscriber<T>> Subscribe<T>(Action<T> callback = null, string channel = null, CancellationToken token = default) where T : IEventData;

        Task<bool> UnSubscribe(ISubscriber callback, CancellationToken token = default);

        IScopedPublisher CreateScope();
    }
    
    public interface IEventData
    {
    }
}
