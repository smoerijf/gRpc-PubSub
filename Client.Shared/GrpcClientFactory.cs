using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace Client.Shared
{
    public interface IGrpcClientFactory
    {
        T CreateClient<T>(Uri remoteEndpoint) where T : class;

        T CreateClient<T>(int remotePort) where T : class => this.CreateClient<T>(new Uri($"http://localhost:{remotePort}"));
    }

    internal class GrpcClientFactory : IGrpcClientFactory
    {
        private readonly Dictionary<Uri, GrpcChannel> grpcChannels = new();
        private readonly HttpClientHandler httpClient;

        public GrpcClientFactory(HttpClientHandler httpClient)
        {
            this.httpClient = httpClient;
        }

        public T CreateClient<T>(Uri remoteUri) where T : class
        {
            var grpcChannel = this.grpcChannels.GetOrCreate(remoteUri, k => GrpcChannel.ForAddress(k, new GrpcChannelOptions { HttpHandler = this.httpClient }));
            return grpcChannel.CreateGrpcService<T>();
        }
    }
}
