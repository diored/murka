using DioRed.Murka.Services;
using DioRed.Vermilion;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

namespace DioRed.Murka.AspLauncher.Services;

public class BroadcastService(VermilionManager botManager, ILogger<BroadcastService> logger) : Broadcast.BroadcastBase
{
    public override async Task<Empty> Agenda(Empty request, ServerCallContext context)
    {
        logger.LogInformation("Someone uses gRPC request: Agenda");
        await botManager.Broadcast(DailyAgenda);
        return new Empty();
    }

    private static async Task DailyAgenda(ChatClient chatClient, CancellationToken token)
    {
        await chatClient.HandleMessageAsync("/agenda", -1, 0, token);
    }
}