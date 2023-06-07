using DioRed.Vermilion;

namespace DioRed.Murka.TelegramBot.Configuration;

public class MurkaConfiguration : BotConfiguration
{
    public required long SuperAdminId { get; init; }
    public required string Version { get; init; }
}
