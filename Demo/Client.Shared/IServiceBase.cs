using System.ServiceModel;
using ProtoBuf.Grpc;

namespace Client.Shared
{
    [ServiceContract]
    public interface IServiceBase
    {
        [OperationContract]
        void Ping(CallContext context = default);
    }
}