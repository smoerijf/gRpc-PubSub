using System.Net;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.Server;

namespace Client.Shared
{
    public static class GrpcServiceExtensions
    {
        public static IServiceCollection AddGrpcConfiguration(this IServiceCollection services)
        {
            services.AddTransientGrpcServiceClient<IClient1Service>(5001);
            services.AddTransientGrpcServiceClient<IClient2Service>(5002);
            
            services.AddSingleton(_ => new HttpClientHandler());
            services.AddCodeFirstGrpc();
            services.AddGrpc(options => options.EnableDetailedErrors = true);
            services.AddSingleton<IGrpcClientFactory, GrpcClientFactory>();
            return services;
        }

        public static void AddTransientGrpcServiceClient<T>(this IServiceCollection services, int port) where T : class
        {
            services.AddTransient(sp =>
            {
                var factory = sp.GetRequiredService<IGrpcClientFactory>();
                return factory.CreateClient<T>(port);
            });
        }

        public static ConfigureWebHostBuilder HostGrpcConfiguration(this ConfigureWebHostBuilder host, int port)
        {
            host.ConfigureKestrel(options => options.Listen(IPAddress.Any, port, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            }));
            return host;
        }
    }
}