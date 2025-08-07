using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands.AdminOnly;

public class AddPromocode(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = "/addPromocode",
        TailPolicy = TailPolicy.HasTail,
        RequiredRole = UserRole.SuperAdmin,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        if (context.Message.Args is not [{ } code, _, { } content])
        {
            return false;
        }

        ServerDateTime? validTo;

        if (context.Message.Args[1] is { } validToString)
        {
            if (!ServerDateTime.TryParse(validToString, out validTo))
            {
                return false;
            }
        }
        else
        {
            validTo = null;
        }

        await logic.AddPromocodeAsync(
            new Promocode(
                code,
                content,
                null,
                validTo
            )
        );

        await feedback.TextAsync($"""
            Promocode has been added:
            - Code: {code}
            - ValidTo: {validTo}
            - Content: {content}
            """
        );

        return true;
    }
}