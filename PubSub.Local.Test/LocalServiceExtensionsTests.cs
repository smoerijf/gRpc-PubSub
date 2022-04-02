using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PubSub.Api;

namespace PubSub.Local.Test;

[TestFixture]
internal class LocalServiceExtensionsTests
{
    [Test]
    public void AddLocalPubSubTest()
    {
        var services = new ServiceCollection();
        services.AddLocalPubSub();

        var provider = services.BuildServiceProvider();
        var ps = provider.GetRequiredService<IPubSub>();

        Assert.IsNotNull(ps, "IPubSub instance");
        Assert.AreEqual(typeof(LocalPubSub), ps.GetType(), "IPubSub type");

        var ps2 = provider.GetRequiredService<IPubSub>();
        Assert.IsTrue(ps == ps2, "IPubSub should be a singleton");
    }
}