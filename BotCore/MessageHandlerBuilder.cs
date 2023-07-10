using DioRed.Murka.Core;
using DioRed.Vermilion;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.BotCore;

public class MessageHandlerBuilder : IMessageHandlerBuilder
{
    private readonly ILogic _logic;
    private readonly ILoggerFactory _logger;

    public MessageHandlerBuilder(ILogic logic, ILoggerFactory logger)
    {
        _logic = logic;
        _logger = logger;
    }

    public IMessageHandler BuildMessageHandler(MessageContext messageContext)
    {
        return new SimpleMessageHandler(messageContext, _logic, _logger);
    }
}