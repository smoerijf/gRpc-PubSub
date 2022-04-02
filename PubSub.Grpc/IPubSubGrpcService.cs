using System;
using System.Collections.Generic;
using System.ServiceModel;
using ProtoBuf;
using ProtoBuf.Grpc;
using PubSub.Api;

namespace PubSub.Grpc
{
    [ServiceContract]
    public interface IPubSubGrpcService
    {
        [OperationContract]
        IAsyncEnumerable<EventData> Subscribe(Guid clientId, IAsyncEnumerable<SubscriptionData> stream, CallContext context = default);
    }

    [ProtoContract]
    public class SubscriptionData
    {
        [ProtoMember(2)]
        public string Channel { get; set; }

        [ProtoMember(3)]
        public Type DataType { get; set; }

        [ProtoMember(4)]
        public Actions Action { get; set; }

        public SubscriptionData()
        {
        }

        public SubscriptionData(string channel, Type dataType, Actions action)
        {
            this.Channel = channel;
            this.DataType = dataType;
            this.Action = action;
        }

        public enum Actions
        {
            Add,
            Remove,
        }
    }

    [ProtoContract]
    public class EventData
    {
        [ProtoMember(1)]
        public string Channel { get; set; }

        [ProtoMember(2)]
        public IEventData Data { get; set; }

        public EventData()
        {
        }

        public EventData(string channel, IEventData data)
        {
            this.Channel = channel;
            this.Data = data;
        }
    }
}
