using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PubSub.Api;

namespace PubSub.Local.Test
{
    [TestFixture]
    internal class Example
    {
        [Test]
        public async Task Test()
        {
            IPubSub pubSub = new LocalPubSub();

            await pubSub.Subscribe<string>(d => Console.WriteLine($"Received: {d}"));

            await pubSub.Publish("Hello world");

            using (var scope = pubSub.CreateScope())
            {
                await scope.Publish("Scoped 1");
                await scope.Commit();

                await scope.Publish("Scoped 2 -- will never be committed.");
            }
        }
    }
}
