using DioRed.Auth.Client;
using DioRed.Common.Jobs;
using DioRed.Murka.Core;
using DioRed.Vermilion;
using DioRed.Vermilion.Telegram;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DioRed.Murka.BotCore;

public static class ServicesExtension
{
    public static IServiceCollection AddMurkaBot(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.AddSingleton(configuration.GetRequiredSection("Vermilion").Get<VermilionConfiguration>()!);
        services.AddSingleton(configuration.GetRequiredSection("Vermilion:Telegram").Get<TelegramBotConfiguration>()!);

        // API
        var authConfiguration = AuthClientConfiguration.Load(configuration.GetRequiredSection("auth"));
        var authClient = new AuthClient(authConfiguration);

        services.AddSingleton(new ApiSettings
        {
            Uri = configuration["apiUri"]!,
            GetAccessToken = authClient.GetAccessToken
        });
        services.AddSingleton<ApiClient>();

        // Storage and Logic
        services.AddSingleton<ILogic, Logic>();
        services.AddSingleton<IChatStorage, ChatStorage>();

        // Bot
        services.AddSingleton<IMessageHandlerBuilder, MessageHandlerBuilder>();
        services.AddSingleton<TelegramVermilionBot>();
        services.AddSingleton<VermilionManager>();

        return services;
    }

    public static IHost UseMurkaBot(this IHost host)
    {
        var vermilionManager = host.Services.GetRequiredService<VermilionManager>();
        var telegramBot = host.Services.GetRequiredService<TelegramVermilionBot>();
        var logic = host.Services.GetRequiredService<ILogic>();
        vermilionManager.AddBot(telegramBot);

        SetupDailyRoutine(logic, vermilionManager);

        vermilionManager.Start();

        return host;
    }


    private static void SetupDailyRoutine(ILogic logic, VermilionManager manager)
    {
        TimeOnly timeToShow = new(21, 0); // 0:00 GMT+3

        var job = Job.SetupDaily(DailyRoutine, timeToShow, "CleanupAndAgenda");
        job.LogInfo += (_, message) => manager.Logger.LogInfo(message);
        job.Start();

        async Task DailyRoutine()
        {
            logic.Cleanup();
            await manager.Broadcast(DailyAgenda);
        }

        async Task DailyAgenda(ChatClient chatClient, CancellationToken token)
        {
            await chatClient.HandleMessageAsync("/agenda", -1, 0, token);
        }
    }
}
