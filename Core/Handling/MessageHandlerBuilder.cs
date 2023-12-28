using DioRed.Vermilion;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core.Handling;

internal class MessageHandlerBuilder(ILogic logic, ILoggerFactory loggerFactory) : IMessageHandlerBuilder
{
    public IMessageHandler BuildMessageHandler(MessageContext messageContext)
    {
        return new SimpleMessageHandler(messageContext, logic, loggerFactory);
    }
}