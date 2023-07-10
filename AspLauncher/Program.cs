using System.Diagnostics;
using System.Runtime.Versioning;

using DioRed.Murka.AspLauncher.Services;
using DioRed.Murka.BotCore;

[assembly: SupportedOSPlatform("windows")]

if (!EventLog.SourceExists("Murka"))
{
    EventLog.CreateEventSource("Murka", "Murka");
};

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddEventLog(settings =>
{
    settings.LogName = "Murka";
    settings.SourceName = "Murka";
});

builder.Services.AddMurkaBot(builder.Configuration);
builder.Services.AddGrpc();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

var app = builder.Build();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.UseCors("AllowAll");

app.MapGrpcService<BroadcastService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.UseMurkaBot();

app.Run();
