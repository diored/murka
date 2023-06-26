using DioRed.Murka.Services;
using DioRed.Vermilion;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

namespace DioRed.Murka.AspLauncher.Services;

public class BroadcastService : Broadcast.BroadcastBase
{
    private readonly VermilionManager _botManager;

    public BroadcastService(VermilionManager botManager)
    {
        _botManager = botManager;
    }

    public override async Task<Empty> Agenda(Empty request, ServerCallContext context)
    {
        await _botManager.Broadcast(DailyAgenda);
        return new Empty();
    }

    private static async Task DailyAgenda(ChatClient chatClient, CancellationToken token)
    {
        await chatClient.HandleMessageAsync("/agenda", -1, 0, token);
    }
}
