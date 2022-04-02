using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PubSub.Api;

namespace PubSub.Local.Test;

[TestFixture]
public class LocalPubSubTests
{
    [Test]
    public async Task SubscribePublishTest([Values(null, "ch1")] string channel)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        await ps.Subscribe<TestData>((ch, d) => received.Add(d.Id), channel);

        await ps.Publish(new TestData { Id = 2 }, channel);
        await ps.Publish(new TestData2 { Id = 3 }, channel);

        Assert.AreEqual(1, received.Count);
        Assert.IsTrue(received.Contains(2));
        Assert.IsFalse(received.Contains(3));
    }

    [Test]
    public async Task UnSubscribeEventPublishTest([Values(null, "ch1")] string channel)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        var sub = await ps.Subscribe<TestData>(null, channel);
        sub.Event += (_, d) => received.Add(d.Id);
        await ps.Publish(new TestData { Id = 2 }, channel);
        await ps.UnSubscribe(sub);
        await ps.Publish(new TestData { Id = 3 }, channel);

        Assert.AreEqual(1, received.Count);
        Assert.IsTrue(received.Contains(2));
        Assert.IsFalse(received.Contains(3));
    }

    [Test]
    public async Task UnSubscribePublishTest([Values(null, "ch1")] string channel)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();
        void fn(string ch, TestData d) => received.Add(d.Id);

        var sub = await ps.Subscribe<TestData>(fn, channel);
        await ps.Publish(new TestData { Id = 2 }, channel);
        await ps.UnSubscribe(sub);
        await ps.Publish(new TestData { Id = 3 }, channel);

        Assert.AreEqual(1, received.Count);
        Assert.IsTrue(received.Contains(2));
        Assert.IsFalse(received.Contains(3));
    }

    [Test]
    public async Task SubscribeCanceled_PublishTest([Values(null, "ch1")] string channel, [Values] bool canceled)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        var sub = await ps.Subscribe<TestData>((ch, d) => received.Add(d.Id), channel, new CancellationToken(canceled));

        await ps.Publish(new TestData { Id = 2 }, channel);

        if (canceled)
        {
            Assert.AreEqual(0, received.Count);
            Assert.IsNull(sub);
        }
        else
        {
            Assert.AreEqual(1, received.Count);
            Assert.IsNotNull(sub);
        }
    }

    [Test]
    public async Task PublishCanceled_SubscribePublishTest([Values(null, "ch1")] string channel, [Values] bool canceled)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        await ps.Subscribe<TestData>((ch, d) => received.Add(d.Id), channel);

        await ps.Publish(new TestData { Id = 2 }, channel);
        await ps.Publish(new TestData { Id = 3 }, channel, new CancellationToken(canceled));

        Assert.AreEqual(canceled ? 1 : 2, received.Count);
        Assert.IsTrue(received.Contains(2));
        Assert.AreEqual(!canceled, received.Contains(3));
    }

    [Test]
    public async Task UnsubscribeCanceled_SubscribePublishTest([Values(null, "ch1")] string channel, [Values] bool canceled)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        var sub = await ps.Subscribe<TestData>((ch, d) => received.Add(d.Id), channel);

        await ps.Publish(new TestData { Id = 2 }, channel);
        await ps.UnSubscribe(sub, new CancellationToken(canceled));
        await ps.Publish(new TestData { Id = 3 }, channel);

        Assert.AreEqual(canceled ? 2 : 1, received.Count);
        Assert.IsTrue(received.Contains(2));
        Assert.AreEqual(canceled, received.Contains(3));
    }
}