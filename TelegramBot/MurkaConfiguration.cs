using DioRed.Murka.Core;

namespace DioRed.Murka.TelegramBot.Configuration;

internal class MurkaConfiguration
{
    public string BotToken { get; set; } = default!;
    public AzureTablesCredentials Azure { get; set; } = default!;
}
