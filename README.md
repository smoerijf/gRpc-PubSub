# gRpc-PubSub
.Net pub/sub library

## Example
Simple example for Local PubSub implementation.
```c#
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

//! Results:
// Received: Hello world
// Received: Scoped 1
```
