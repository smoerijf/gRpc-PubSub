using Client.Shared;
using Microsoft.AspNetCore.Builder;

Console.WriteLine("Hello, World, I'm client2!");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.HostGrpcConfiguration(5002);
builder.Services.AddGrpcConfiguration();

var app = builder.Build();
app.MapGrpcService<Client2Service>();

app.Run();