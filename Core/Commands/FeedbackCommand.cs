using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands;

public class FeedbackCommand : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = "/feedback",
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        if (string.IsNullOrWhiteSpace(context.Message.Tail))
        {
            await feedback.TextAsync("Добавьте текст обратной связи после команды /feedback, и админ получит ваше сообщение.");
            return true;
        }

        await feedback.To(chatInfo => context.Chat.Connector.IsSuperAdmin(chatInfo.ChatId)).TextAsync(
            $"""
            Feedback from "{context.Sender.Name}" (id: #{context.Sender.Id}):
            {context.Message.Tail}
            """
        );

        await feedback.TextAsync("Спасибо за обратную связь, сообщение отправлено админу!");

        return true;
    }
}