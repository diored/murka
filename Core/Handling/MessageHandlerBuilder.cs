using DioRed.Vermilion;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core.Handling;

internal class MessageHandlerBuilder : IMessageHandlerBuilder
{
    private readonly ILogic _logic;
    private readonly ILoggerFactory _loggerFactory;

    public MessageHandlerBuilder(ILogic logic, ILoggerFactory loggerFactory)
    {
        _logic = logic;
        _loggerFactory = loggerFactory;
    }

    public IMessageHandler BuildMessageHandler(MessageContext messageContext)
    {
        return new SimpleMessageHandler(messageContext, _logic, _loggerFactory);
    }
}