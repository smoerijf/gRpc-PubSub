using Client.Shared;
using Grpc.Core;
using Microsoft.Extensions.Hosting;

namespace Client1
{
    public class PingerService : BackgroundService
    {
        private readonly IClient2Service client2;

        public PingerService(IClient2Service client2)
        {
            this.client2 = client2;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.ConnectToClient2(stoppingToken);
            Console.WriteLine($"Connected to client2.");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Pinging client2!");
                this.client2.Ping();
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task ConnectToClient2(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    this.client2.Ping();
                    break;
                }
                catch (RpcException e)
                {
                    Console.WriteLine($"Connect to client2 error: {e}");
                }

                await Task.Delay(200, stoppingToken);
            }
        }
    }
}
