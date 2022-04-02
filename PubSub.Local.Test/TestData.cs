using PubSub.Api;

namespace PubSub.Local.Test;

internal class TestData : IEventData
{
    public int Id { get; init; }
}

internal class TestData2 : IEventData
{
    public int Id { get; init; }
}