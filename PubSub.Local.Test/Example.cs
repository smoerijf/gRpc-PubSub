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

            await pubSub.Subscribe<Data>((ch, d) => Console.WriteLine($"Received[{ch}]: {d.Value}"));

            await pubSub.Publish(new Data("Hello world"));

            using (var scope = pubSub.CreateScope())
            {
                await scope.Publish(new Data("Scoped 1"));
                await scope.Commit();

                await scope.Publish(new Data("Scoped 2 -- will never be committed."));
            }
        }

        public class Data : IEventData
        {
            public string Value { get; set; }

            public Data(string value) => this.Value = value;
        }
    }
}
