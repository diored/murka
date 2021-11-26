using DioRed.Murka.Core;
using DioRed.Murka.Core.Entities;

using Telegram.Bot;

namespace DioRed.Murka.TelegramBot;

public class ChatClient
{
    private readonly long _chatId;

    private readonly object _agendaLock = new();
    private bool _subscribedForAgenda;
    private CancellationToken _cancelAgenda;

    private readonly ClientData _clientData;

    private readonly IDataSource _dataSource;

    public ChatClient(long chatId, IDataSource dataSource)
    {
        _chatId = chatId;
        _dataSource = dataSource;

        _clientData = new ClientData();
    }

    public async Task HandleMessage(ITelegramBotClient botClient, string message, CancellationToken cancellationToken)
    {
        var context = new HandleContext(botClient, _chatId, _dataSource, cancellationToken);
        await new MessageHandler(context).Handle(message, _clientData);

        CheckAgendaSubscription(botClient, cancellationToken);
    }

    private void CheckAgendaSubscription(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        lock (_agendaLock)
        {
            if (_subscribedForAgenda)
            {
                return;
            }

            _cancelAgenda = new CancellationTokenSource().Token;

            _ = Task.Run(async () =>
            {
                var context = new HandleContext(botClient, _chatId, _dataSource, cancellationToken);
                var messageHandler = new MessageHandler(context);

                while (true)
                {
                    // Time to next noon
                    DateTime serverTime = ServerTime.GetCurrent();
                    DateTime scheduleTo = serverTime.Date.AddDays(1).AddSeconds(5);

                    TimeSpan interval = scheduleTo - serverTime;
                    if (interval.TotalDays > 1)
                    {
                        interval -= TimeSpan.FromDays(1);
                    }

                    await Task.Delay(interval);
                    await messageHandler.Handle("/agenda", _clientData);
                }
            }, _cancelAgenda);

            _subscribedForAgenda = true;
        }
    }
}