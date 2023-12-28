using DioRed.Common.Logging;
using DioRed.Murka.AspLauncher.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.SetupDioRedLogging("Murka");

builder.Services
    .AddMurkaBot(builder.Configuration)
    .AddGrpc().Services
    .AddCors(o => o.AddPolicy("AllowAll", builder =>
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

app.Services.UseMurkaBot();

app.Run();