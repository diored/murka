using DioRed.Api.Client;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

namespace DioRed.Murka.Core;

internal class ApiFacade(ApiRequestBuilder api)
{
    public async Task<ICollection<ChatId>> GetChats()
    {
        var response = await api.Create("chats/get").GetAsync();

        return await response.AsAsync<ICollection<ChatId>>();
    }

    public async Task AddChat(BotSystem system, string type, long id, string title)
    {
        var response = await api.Create("chats/add")
            .WithBody(new { system = system.ToString(), type, id, title })
            .PostAsync();

        response.EnsureSuccess();
    }

    public async Task RemoveChat(BotSystem system, long id)
    {
        var response = await api.Create("chats/remove")
            .WithBody(new { system = system.ToString(), id })
            .PostAsync();

        response.EnsureSuccess();
    }

    public async Task<Daily> GetDaily(string date)
    {
        var response = await api.Create("daily/get")
            .WithQueryArgs(new { date })
            .GetAsync();

        return await response.AsAsync<Daily>();
    }

    public async Task SetDailyMonth(int month, string dailies)
    {
        var response = await api.Create("daily/setMonth")
            .WithBody(new { month, dailies })
            .PostAsync();

        response.EnsureSuccess();
    }

    public async Task<ICollection<DayEvent>> GetDayEvents(string date, ChatId chatId)
    {
        var response = await api.Create("dayevents/get")
            .WithQueryArgs(new
            {
                date,
                chatSystem = chatId.System,
                chatId = chatId.Id
            })
            .GetAsync();

        return await response.AsAsync<ICollection<DayEvent>>();
    }

    public async Task AddDayEvent(string name, string occurrence, string time, ChatId? chatId)
    {
        var response = await api.Create("dayevents/add")
            .WithBody(new { name, occurrence, time, chatId })
            .PostAsync();

        response.EnsureSuccess();
    }

    public async Task<ICollection<Event>> GetActiveEvents()
    {
        var response = await api.Create("events/active").GetAsync();

        return await response.AsAsync<ICollection<Event>>();
    }

    public async Task AddEvent(string name, string? validFrom, string? validTo)
    {
        var response = await api.Create("events/add")
            .WithBody(new { name, validFrom, validTo })
            .PostAsync();

        response.EnsureSuccess();
    }

    public async Task CleanupEvents()
    {
        var response = await api.Create("events/cleanup").PostAsync();

        response.EnsureSuccess();
    }

    public async Task<string> GetRandomGreeting()
    {
        var response = await api.Create("greeting").GetAsync();

        return await response.AsStringAsync();
    }

    public async Task<Northlands> GetNorthlands(string date)
    {
        var response = await api.Create("north/get")
            .WithQueryArgs(new { date })
            .GetAsync();

        return await response.AsAsync<Northlands>();
    }

    public async Task<ICollection<Promocode>> GetActivePromocodes()
    {
        var response = await api.Create("promocodes/active").GetAsync();

        return await response.AsAsync<ICollection<Promocode>>();
    }

    public async Task AddPromocode(string code, string? validFrom, string? validTo, string content)
    {
        var response = await api.Create("promocodes/add")
            .WithBody(new { code, validFrom, validTo, content })
            .PostAsync();

        response.EnsureSuccess();
    }

    public async Task UpdatePromocode(string code, string? validFrom, string? validTo, string content)
    {
        var response = await api.Create("promocodes/update")
            .WithBody(new { code, validFrom, validTo, content })
            .PostAsync();

        response.EnsureSuccess();
    }

    public async Task RemovePromocode(string code)
    {
        var response = await api.Create("promocodes/remove")
            .WithBody(new { code })
            .PostAsync();

        response.EnsureSuccess();
    }

    public async Task CleanupPromocodes()
    {
        var response = await api.Create("promocodes/cleanup").PostAsync();

        response.EnsureSuccess();
    }

    public async Task<string> GetLink(string id)
    {
        var response = await api.Create($"links/{id}").GetAsync();

        return response.AsString();
    }
}