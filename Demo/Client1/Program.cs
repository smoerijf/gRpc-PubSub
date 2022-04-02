using Client.Shared;
using Client1;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PubSub.Grpc;

Console.WriteLine("Hello, World, I'm client1!");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.HostGrpcConfiguration(5001);
builder.Services.AddGrpcConfiguration();
builder.Services.AddGrpcPubSub();
builder.Services.AddHostedService<PingerService>();
builder.Services.AddHostedService<RandomEventGenerator>();

var app = builder.Build();
app.MapGrpcService<Client1Service>();
app.MapGrpcService<PubSubGrpcService>();

app.Run();
