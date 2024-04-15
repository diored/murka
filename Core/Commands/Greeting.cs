using System.Text.RegularExpressions;

using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands;

public partial class Greeting(
    ILogic logic
) : ICommandHandler
{
    private readonly TimeSpan _greetingInterval = TimeSpan.FromMinutes(40);

    public CommandDefinition Definition { get; } = new()
    {
        Template = GreetingRegex(),
        Priority = CommandPriority.Low,
        LogHandling = false
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        if (context.Chat.Properties.GetValueOrDefault("LatestGreeting") is DateTimeOffset latestGreeting &&
            DateTimeOffset.UtcNow - latestGreeting < _greetingInterval)
        {
            return false;
        }

        context.Chat.Properties["LatestGreeting"] = DateTimeOffset.UtcNow;
        string greeting = await logic.GetRandomGreetingAsync();

        await feedback.TextAsync(greeting);

        return true;
    }

    [GeneratedRegex("(?:привет|доброе утро|добрый день|добрый вечер|здравствуйте)")]
    private static partial Regex GreetingRegex();
}