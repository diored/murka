using DioRed.Murka.Core.Entities;
using DioRed.Murka.Infrastructure.Models;

namespace DioRed.Murka.Core.Modules;

public interface IDailyModule
{
    Daily Get(DateOnly date);
    FileResult? GetCalendar();
}