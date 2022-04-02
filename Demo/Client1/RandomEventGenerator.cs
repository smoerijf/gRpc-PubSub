using Client.Shared;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using PubSub.Api;

namespace Client1
{
    public class RandomEventGenerator : BackgroundService
    {
        private readonly IPubSub pubSub;

        public RandomEventGenerator(IPubSub pubSub)
        {
            this.pubSub = pubSub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var random = new Random();
            while (!stoppingToken.IsCancellationRequested)
            {
                var r = random.Next(3000, 6000);
                await Task.Delay(r, stoppingToken);
                Console.WriteLine("Its randomly time to Publish something from Client1!");
                await this.pubSub.Publish(new TestEventData($"XYZ {r}"), token: stoppingToken);
            }
        }
    }
}
