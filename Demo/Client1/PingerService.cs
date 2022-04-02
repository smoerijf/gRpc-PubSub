using Client.Shared;
using Grpc.Core;
using Microsoft.Extensions.Hosting;

namespace Client1
{
    public class PingerService : BackgroundService
    {
        private readonly IClient2Service client1;

        public PingerService(IClient2Service client1)
        {
            this.client1 = client1;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    this.client1.Ping();
                    break;
                }
                catch (RpcException e)
                {
                    Console.WriteLine($"Connect to client2 error: {e}");
                }
                await Task.Delay(200, stoppingToken);
            }
            Console.WriteLine($"Connected to client2.");

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Pinging client2!");
                this.client1.Ping();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
