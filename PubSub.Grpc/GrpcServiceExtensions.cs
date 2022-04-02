using System;
using Microsoft.Extensions.DependencyInjection;
using PubSub.Api;

namespace PubSub.Grpc
{
    public static class GrpcServiceExtensions
    {
        public static IServiceCollection AddGrpcPubSub(this IServiceCollection services)
        {
            services.AddTransient<IPubSub, GrpcPubSub>();
            services.AddSingleton<IServerSubscriptionFactory, ServerSubscriptionFactory>();
            services.AddSingleton<IClientSubscriptionFactory, ClientSubscriptionFactory>();
            services.AddTransient<IClientCommunicator, ClientCommunicator>();
            services.AddTransient<Func<IClientCommunicator>>(sp => () => sp.GetRequiredService<IClientCommunicator>());
            //services.MapGrpcService<PubSubGrpcService>();
            return services;
        }
    }
}
