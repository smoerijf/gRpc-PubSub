using Client.Shared;
using Microsoft.AspNetCore.Builder;
using PubSub.Grpc;

Console.WriteLine("Hello, World, I'm client2!");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.HostGrpcConfiguration(5002);
builder.Services.AddGrpcPubSub();
builder.Services.AddGrpcConfiguration();

var app = builder.Build();
app.MapGrpcService<Client2Service>();
app.MapGrpcService<PubSubGrpcService>();

app.Run();