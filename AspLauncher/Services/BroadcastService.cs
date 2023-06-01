using DioRed.Murka.Services;
using DioRed.Murka.TelegramBot;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

namespace DioRed.Murka.AspLauncher.Services;

public class BroadcastService : Broadcast.BroadcastBase
{
    private readonly MurkaBot _murkaBot;

    public BroadcastService(MurkaBot murkaBot)
    {
        _murkaBot = murkaBot;
    }

    public override async Task<Empty> Agenda(Empty request, ServerCallContext context)
    {
        await _murkaBot.Broadcast(_murkaBot.DailyAgenda);
        return new Empty();
    }
}
