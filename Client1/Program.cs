using Client.Shared;
using Client1;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World, I'm client1!");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.HostGrpcConfiguration(5001);
builder.Services.AddGrpcConfiguration();
builder.Services.AddHostedService<PingerService>();

var app = builder.Build();
app.MapGrpcService<Client1Service>();

app.Run();
