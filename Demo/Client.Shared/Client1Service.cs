using ProtoBuf.Grpc;

namespace Client.Shared
{
    public interface IClient1Service : IServiceBase
    {
    }

    public class Client1Service : IClient1Service
    {
        public void Ping(CallContext context = default)
        {
            Console.WriteLine("Client1::ping received");
        }
    }
}
