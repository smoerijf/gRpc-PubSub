using Microsoft.Extensions.DependencyInjection;
using PubSub.Api;

namespace PubSub.Local
{
    public static class LocalServiceExtensions
    {
        public static IServiceCollection AddLocalPubSub(this IServiceCollection services)
        {
            services.AddSingleton<IPubSub, LocalPubSub>();
            return services;
        }
    }
}
