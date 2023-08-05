using DioRed.Murka.Services;
using DioRed.Vermilion;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

namespace DioRed.Murka.AspLauncher.Services;

public class BroadcastService : Broadcast.BroadcastBase
{
    private readonly VermilionManager _botManager;
    private readonly ILogger _logger;

    public BroadcastService(VermilionManager botManager, ILogger<BroadcastService> logger)
    {
        _botManager = botManager;
        _logger = logger;
    }

    public override async Task<Empty> Agenda(Empty request, ServerCallContext context)
    {
        _logger.LogInformation("Someone uses gRPC request: Agenda");
        await _botManager.Broadcast(DailyAgenda);
        return new Empty();
    }

    private static async Task DailyAgenda(ChatClient chatClient, CancellationToken token)
    {
        await chatClient.HandleMessageAsync("/agenda", -1, 0, token);
    }
}