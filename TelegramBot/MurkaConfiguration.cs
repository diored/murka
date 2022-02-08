using DioRed.Murka.Core;
using DioRed.Vermilion;

namespace DioRed.Murka.TelegramBot.Configuration;

public class MurkaConfiguration : BotConfiguration
{
    public AzureTablesCredentials Azure { get; set; } = default!;
    public long AdminId { get; set; } = default!;
}
