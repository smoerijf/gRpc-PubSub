using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PubSub.Api;
using PubSub.Local;

namespace PubSub.Local.Test;

public class ScopedPubSubTests
{
    [Test]
    public async Task ScopeDisposed_NothingReceived([Values(null, "ch1")] string channel)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        await ps.Subscribe<TestData>(d => received.Add(d.Id), channel);

        using (var scope = ps.CreateScope())
        {
            await scope.Publish(new TestData { Id = 2 }, channel);
        }

        Assert.AreEqual(0, received.Count);
    }

    [Test]
    public async Task ScopeCommit_OnlyReceivedWhatWasCommitted([Values(null, "ch1")] string channel)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        await ps.Subscribe<TestData>(d => received.Add(d.Id), channel);

        using (var scope = ps.CreateScope())
        {
            await scope.Publish(new TestData { Id = 2 }, channel);
            await scope.Commit();
            await scope.Publish(new TestData { Id = 3 }, channel);
        }

        Assert.AreEqual(1, received.Count);
        Assert.IsTrue(received.Contains(2));
        Assert.IsFalse(received.Contains(3));
    }

    [Test]
    public async Task TokenCancelForPublish_DataShouldNotBeReceived([Values(null, "ch1")] string channel)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        await ps.Subscribe<TestData>(d => received.Add(d.Id), channel);

        using (var scope = ps.CreateScope())
        {
            await scope.Publish(new TestData { Id = 2 }, channel);
            await scope.Publish(new TestData { Id = 3 }, channel, new CancellationToken(true));
            await scope.Commit();
        }

        Assert.AreEqual(1, received.Count);
        Assert.IsTrue(received.Contains(2));
        Assert.IsFalse(received.Contains(3));
    }

    [Test]
    public async Task TokenCancelForCommit_DataShouldNotBeReceived([Values(null, "ch1")] string channel)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        await ps.Subscribe<TestData>(d => received.Add(d.Id), channel);

        using (var scope = ps.CreateScope())
        {
            await scope.Publish(new TestData { Id = 2 }, channel);
            await scope.Commit();
            await scope.Publish(new TestData { Id = 3 }, channel);
            await scope.Commit(new CancellationToken(true));
        }

        Assert.AreEqual(1, received.Count);
        Assert.IsTrue(received.Contains(2));
        Assert.IsFalse(received.Contains(3));
    }

    [Test]
    public async Task ScopedCommit_ReceivedWCommittedAndGlobal([Values(null, "ch1")] string channel)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        await ps.Subscribe<TestData>(d => received.Add(d.Id), channel);

        await ps.Publish(new TestData { Id = 1 }, channel);
        using (var scope = ps.CreateScope())
        {
            await scope.Publish(new TestData { Id = 2 }, channel);
            await scope.Commit();
            await ps.Publish(new TestData { Id = 3 }, channel);
        }

        Assert.AreEqual(3, received.Count);
        Assert.IsTrue(received.Contains(1));
        Assert.IsTrue(received.Contains(2));
        Assert.IsTrue(received.Contains(3));
    }

    [Test]
    public async Task PublishToDisposedThrows([Values(null, "ch1")] string channel)
    {
        IPubSub ps = new LocalPubSub();
        var received = new List<int>();

        await ps.Subscribe<TestData>(d => received.Add(d.Id), channel);

        using (var scope = ps.CreateScope())
        {
            scope.Dispose();
            Assert.ThrowsAsync<InvalidOperationException>(async () => await scope.Publish(new TestData { Id = 2 }, channel));
            await scope.Commit();
        }

        Assert.AreEqual(0, received.Count);
    }
}