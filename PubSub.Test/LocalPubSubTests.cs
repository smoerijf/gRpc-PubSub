using System.Collections.Generic;
using NUnit.Framework;
using PubSub.Api;
using PubSub.Local;

namespace PubSub.Test
{
    public class LocalPubSubTests
    {
        [Test]
        public void SubscribePublishTest([Values(null, "ch1")] string channel)
        {
            var ps = new LocalPubSub();
            var received = new List<int>();

            ps.Subscribe<TestData>(d => received.Add(d.Id), channel);

            ps.Publish(new TestData { Id = 2 }, channel);
            ps.Publish(new TestData2 { Id = 3 }, channel);

            Assert.AreEqual(1, received.Count);
            Assert.IsTrue(received.Contains(2));
            Assert.IsFalse(received.Contains(3));
        }

        [Test]
        public void UnSubscribeEventPublishTest([Values(null, "ch1")] string channel)
        {
            var ps = new LocalPubSub();
            var received = new List<int>();

            var sub = ps.Subscribe<TestData>(null, channel);
            sub.Event += d => received.Add(d.Id);
            ps.Publish(new TestData { Id = 2 }, channel);
            ps.UnSubscribe(sub);
            ps.Publish(new TestData { Id = 3 }, channel);

            Assert.AreEqual(1, received.Count);
            Assert.IsTrue(received.Contains(2));
            Assert.IsFalse(received.Contains(3));
        }

        [Test]
        public void UnSubscribePublishTest([Values(null, "ch1")] string channel)
        {
            var ps = new LocalPubSub();
            var received = new List<int>();
            void fn(TestData d) => received.Add(d.Id);

            var sub = ps.Subscribe<TestData>(fn, channel);
            ps.Publish(new TestData { Id = 2 }, channel);
            ps.UnSubscribe(sub);
            ps.Publish(new TestData { Id = 3 }, channel);

            Assert.AreEqual(1, received.Count);
            Assert.IsTrue(received.Contains(2));
            Assert.IsFalse(received.Contains(3));
        }

        private class TestData : IEventData
        {
            public int Id { get; init; }
        }

        private class TestData2 : IEventData
        {
            public int Id { get; init; }
        }
    }
}