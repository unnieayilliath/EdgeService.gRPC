using EdgeService.gRPC.CloudConnector;
using EdgeService.gRPC.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        listenOptions.UseHttps();
    });
});
// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<IoTMessageService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
//CloudConnector _cloudConnector= new CloudConnector();
//Timestamp receivedTime = DateTime.UtcNow.ToTimestamp();
//var cloudRequest = new CloudRequest { Data = "unnie", SendTime = receivedTime, EdgeReceivedTime = receivedTime };
//await _cloudConnector.SendToCloudAsync(cloudRequest);