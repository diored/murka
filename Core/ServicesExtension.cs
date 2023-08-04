using DioRed.Auth.Client;
using DioRed.Common.Jobs;
using DioRed.Murka.Core.Handling;
using DioRed.Vermilion;
using DioRed.Vermilion.Telegram;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core;

public static class ServicesExtension
{
    public static IServiceCollection AddMurkaBot(this IServiceCollection services, IConfiguration configuration)
    {
        // API
        var authConfiguration = AuthClientConfiguration.Load(configuration.GetRequiredSection("auth"));
        var authClient = new AuthClient(authConfiguration);

        services.AddSingleton(new ApiSettings
        {
            Uri = configuration["apiUri"]!,
            GetAccessToken = authClient.GetAccessToken
        });
        services.AddSingleton<ApiClient>();

        // Handling
        services.AddSingleton<ILogic, Logic>();
        services.AddSingleton<IChatStorage, ChatStorage>();
        services.AddSingleton<IMessageHandlerBuilder, MessageHandlerBuilder>();

        // Vermilion
        services.AddSingleton(configuration.GetRequiredSection("Vermilion").Get<VermilionConfiguration>()!);
        services.AddSingleton(configuration.GetRequiredSection("Vermilion:Telegram").Get<TelegramBotConfiguration>()!);
        services.AddSingleton<TelegramVermilionBot>();
        services.AddSingleton<VermilionManager>();

        return services;
    }

    public static IServiceProvider UseMurkaBot(this IServiceProvider services)
    {
        var vermilionManager = services.GetRequiredService<VermilionManager>();
        var telegramBot = services.GetRequiredService<TelegramVermilionBot>();
        var logic = services.GetRequiredService<ILogic>();
        vermilionManager.AddBot(telegramBot);

        var loggerFactory = services.GetRequiredService<ILoggerFactory>();

        SetupDailyRoutine(logic, vermilionManager, loggerFactory.CreateLogger("Murka.Jobs"));

        vermilionManager.Start();

        return services;
    }

    private static void SetupDailyRoutine(ILogic logic, VermilionManager manager, ILogger logger)
    {
        TimeOnly timeToShow = new(21, 0); // 0:00 GMT+3

        var job = Job.SetupDaily(DailyRoutine, timeToShow, "CleanupAndAgenda");
        job.LogInfo += (_, message) => logger.LogInformation(message);
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