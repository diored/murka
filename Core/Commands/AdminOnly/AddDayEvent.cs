using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;

namespace DioRed.Murka.Core.Commands.AdminOnly;

public class AddDayEvent(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = new[] { "/addDayEvent", "/addDayEvent!" },
        HasTail = true,
        RequiredRole = UserRole.ChatAdmin,
        LogHandling = true
    };

    /*
    Добавлять ивенты может только администратор чата

    Синтаксис:
        <code>/addDayEvent {наименование}|{время}|{повторение}</code>

        Повторение:
        <code>daily</code> — ежедневно
        <code>1</code> — по понедельникам
        <code>2</code> — по вторникам
        ...
        <code>7</code> — по воскресеньям

        Пример:
        <code>/addDayEvent Клан-холл|17:30|6</code>

        <b>Примечание</b>
        Удаление ранее добавленных событий будет реализовано в одной из последующих версий.
    */

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        if (context.Message.Args is not [{ } name, { } timeString, { } occurrenceString])
        {
            return false;
        }

        bool isGlobal = context.Message.Command.EndsWith('!');

        if (isGlobal && !context.Sender.Role.HasFlag(UserRole.SuperAdmin))
        {
            return false;
        }

        if (!TimeOnly.TryParseExact(timeString, "HH:mm", out TimeOnly time))
        {
            return false;
        }

        string? occurrence = occurrenceString.ToLowerInvariant() switch
        {
            "daily" => "daily",
            "mo" or "monday" or "1" => "weekly:1",
            "tu" or "tuesday" or "2" => "weekly:2",
            "we" or "wednesday" or "3" => "weekly:3",
            "th" or "thursday" or "4" => "weekly:4",
            "fr" or "friday" or "5" => "weekly:5",
            "sa" or "saturday" or "6" => "weekly:6",
            "su" or "sunday" or "7" or "0" => "weekly:0",
            _ => null
        };

        if (occurrence is null)
        {
            return false;
        }

        await logic.AddDayEventAsync(
            name,
            occurrence,
            time,
            isGlobal ? null : context.Chat.Id
        );

        await feedback.TextAsync($"""
            Day event has been added:
            - Name: {name}
            - Occurrence: {occurrence}
            - Time: {time}
            """
        );

        return true;
    }
}