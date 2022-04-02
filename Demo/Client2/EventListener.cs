using Client.Shared;
using Microsoft.Extensions.Hosting;
using PubSub.Api;

namespace Client1
{
    public class EventListener : BackgroundService
    {
        private readonly IPubSub pubSub;

        public EventListener(IPubSub pubSub)
        {
            this.pubSub = pubSub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var sub = await this.pubSub.Subscribe<TestEventData>(token: stoppingToken);
            sub.Event += this.OnEvent;

            var sub2 = await this.pubSub.Subscribe<TestEventData>(this.OnEvent, "abc", stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }

            await this.pubSub.UnSubscribe(sub, default);
            await this.pubSub.UnSubscribe(sub2, default);
        }

        private void OnEvent(string channel, TestEventData eventData)
        {
            Console.WriteLine($"PubSub[{channel}]: {eventData}");
        }
    }
}
