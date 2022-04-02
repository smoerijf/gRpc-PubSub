using System.ServiceModel;
using ProtoBuf.Grpc;

namespace Client.Shared
{
    [ServiceContract]
    public interface IClient1Service : IServiceBase
    {
    }

    public class Client1Service : IClient1Service
    {
        public Client1Service()
        {
            Console.WriteLine("Client1Service::created");
        }

        public void Ping(CallContext context = default)
        {
            Console.WriteLine("Client1::ping received");
        }
    }
}
