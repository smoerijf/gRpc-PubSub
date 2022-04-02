using PubSub.Api;

namespace Client.Shared
{
    public class TestEventData : IEventData
    {
        public string Value { get; set; }

        public TestEventData(string value) => this.Value = value;

        public override string ToString() => $"{this.Value}";
    }
}
