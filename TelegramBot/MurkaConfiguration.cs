using DioRed.Vermilion;

namespace DioRed.Murka.TelegramBot.Configuration;

public class MurkaConfiguration : BotConfiguration
{
    public long SuperAdminId { get; set; } = default!;
}
