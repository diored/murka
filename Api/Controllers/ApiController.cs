using DioRed.Murka.Core;
using DioRed.Murka.Core.Entities;

using Microsoft.AspNetCore.Mvc;

namespace DioRed.Murka.Api;

[Route("api")]
[ApiController]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class ApiController : ControllerBase
{
    private readonly IDataSource _dataSource;

    public ApiController(IDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    [HttpGet("activeEvents/{dateTime}")]
    public Event[] GetActiveEvents(DateTime dateTime)
    {
        return _dataSource.GetActiveEvents(dateTime);
    }

    [HttpGet("activePromocodes/{dateTime}")]
    public Promocode[] GetActivePromocodes(DateTime dateTime)
    {
        return _dataSource.GetActivePromocodes(dateTime);
    }

    [HttpGet("daily/{dateTime}")]
    public Daily? GetDaily(DateTime dateTime)
    {
        return _dataSource.GetDaily(dateTime);
    }

    [HttpGet("dayEvents/{dateTime}")]
    public IEnumerable<string> GetDayEvents(DateTime dateTime)
    {
        return _dataSource.GetDayEvents(dateTime);
    }

    [HttpGet("north/{dateTime}/{army}")]
    public string GetNorth(DateTime dateTime, NorthArmy army)
    {
        return _dataSource.GetNorth(dateTime, army);
    }

    [HttpGet("randomGreeting")]
    public string GetRandomGreeting()
    {
        return _dataSource.GetRandomGreeting();
    }
}