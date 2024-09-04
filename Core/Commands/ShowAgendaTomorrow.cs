using DioRed.Murka.Core.Entities;
using DioRed.Vermilion.Handling.Templates;

namespace DioRed.Murka.Core.Commands;

public class ShowAgendaTomorrow(ILogic logic)
    : ShowAgendaBase(logic, template, getDateFunc)
{
    private static readonly Template template = new[] { "/tomorrow", "завтра" };
    private static readonly Func<DateOnly> getDateFunc = () => ServerDateTime.GetCurrent().Date.AddDays(1);
}