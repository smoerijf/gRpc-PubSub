using ProtoBuf.Grpc;

namespace Client.Shared
{
    public interface IClient2Service : IServiceBase
    {
    }

    public class Client2Service : IClient2Service
    {
        public void Ping(CallContext context = default)
        {
            Console.WriteLine("Client2::ping received");
        }
    }
}