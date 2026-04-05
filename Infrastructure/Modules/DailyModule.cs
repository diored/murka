using Azure;

using DioRed.Murka.Core.Entities;
using DioRed.Murka.Core.Modules;
using DioRed.Murka.Infrastructure.AzureStorage;
using DioRed.Murka.Infrastructure.Models;

namespace DioRed.Murka.Infrastructure.Modules;

public class DailyModule(ImagesStorage images, ParametersStorage parameters) : IDailyModule
{
    public Daily Get(DateOnly date)
    {
        Daily daily = ((date.DayNumber + parameters.GetInt32("DailyOffset")) % 3) switch
        {
            0 => Daily.Weapon,
            1 => Daily.Armor,
            2 => Daily.Relic,
            _ => default!
        };

        return daily;
    }

    public FileResult? GetCalendar()
    {
        BinaryData data;
        string contentType;
        try
        {
            data = images.Get("daily.jpg");
            contentType = "image/jpeg";
        }
        catch (RequestFailedException)
        {
            data = images.Get("daily.png");
            contentType = "image/png";
        }
        catch
        {
            return null;
        }

        return new FileResult
        {
            Content = data.ToArray(),
            ContentType = contentType
        };
    }
}