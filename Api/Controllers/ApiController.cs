using Azure.Data.Tables;

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
    private readonly TableClient _chatMembersTable;
    private readonly TableClient _promocodesTable;

    public ApiController(IDataSource dataSource, IConfiguration configuration)
    {
        _dataSource = dataSource;

        string storageUri = configuration["data:uri"];
        string accountName = configuration["data:account"];
        string storageAccountKey = configuration["data:key"];

        Uri endpoint = new(storageUri);
        TableSharedKeyCredential credential = new(accountName, storageAccountKey);

        _chatMembersTable = new(endpoint, "Chats", credential);
        _promocodesTable = new(endpoint, "Promocodes", credential);
    }

    [HttpGet("activeEvents/{dateTime}")]
    public Event[] GetActiveEvents(DateTime dateTime)
    {
        return _dataSource.GetActiveEvents(dateTime);
    }

    [HttpGet("activePromocodes/{dateTime}")]
    public Promocode[] GetActivePromocodes(DateTime dateTime)
    {
        return _promocodesTable
            .Query<TableEntities.Promocode>(p => p.ValidTo > dateTime)
            .Select(entity => new Promocode(entity.ValidTo, entity.RowKey, entity.Content))
            .ToArray();
    }

    [HttpGet("daily/{dateTime}")]
    public Daily GetDaily(DateTime dateTime)
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