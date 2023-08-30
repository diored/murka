using DioRed.Api.Client;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

namespace DioRed.Murka.Core;

internal class ApiFacade
{
    private readonly HttpApiClient _http;

    public ApiFacade(HttpApiClient httpApiClient)
    {
        _http = httpApiClient;
    }

    public async Task<ICollection<ChatId>> GetChats()
    {
        return await _http.GetAsync<ICollection<ChatId>>("chats/get");
    }

    public async Task AddChat(BotSystem system, string type, long id, string title)
    {
        await _http.PostAsync("chats/add", new { system = system.ToString(), type, id, title });
    }

    public async Task RemoveChat(BotSystem system, long id)
    {
        await _http.PostAsync("chats/remove", new { system = system.ToString(), id });
    }

    public async Task<Daily> GetDaily(string date)
    {
        return await _http.GetAsync<Daily>("daily/get", new { date });
    }

    public async Task SetDailyMonth(int month, string dailies)
    {
        await _http.PostAsync("daily/setMonth", new { month, dailies });
    }

    public async Task<ICollection<DayEvent>> GetDayEvents(string date, ChatId chatId)
    {
        return await _http.GetAsync<ICollection<DayEvent>>("dayevents/get", new
        {
            date,
            chatSystem = chatId.System,
            chatId = chatId.Id
        });
    }

    public async Task AddDayEvent(string name, string occurrence, string time, ChatId? chatId)
    {
        await _http.PostAsync("dayevents/add", new { name, occurrence, time, chatId });
    }

    public async Task<ICollection<Event>> GetActiveEvents()
    {
        return await _http.GetAsync<ICollection<Event>>("events/active");
    }

    public async Task AddEvent(string name, string? validFrom, string? validTo)
    {
        await _http.PostAsync("events/add", new { name, validFrom, validTo });
    }

    public async Task CleanupEvents()
    {
        await _http.PostAsync("events/cleanup");
    }

    public async Task<string> GetRandomGreeting()
    {
        return await _http.GetAsync<string>("greeting");
    }

    public async Task<Northlands> GetNorthlands(string date)
    {
        return await _http.GetAsync<Northlands>("north/get", new { date });
    }

    public async Task<ICollection<Promocode>> GetActivePromocodes()
    {
        return await _http.GetAsync<ICollection<Promocode>>("promocodes/active");
    }

    public async Task AddPromocode(string code, string? validFrom, string? validTo, string content)
    {
        await _http.PostAsync("promocodes/add", new { code, validFrom, validTo, content });
    }

    public async Task UpdatePromocode(string code, string? validFrom, string? validTo, string content)
    {
        await _http.PostAsync("promocodes/update", new { code, validFrom, validTo, content });
    }

    public async Task RemovePromocode(string code)
    {
        await _http.PostAsync("promocodes/remove", new { code });
    }

    public async Task CleanupPromocodes()
    {
        await _http.PostAsync("promocodes/cleanup");
    }

    public async Task<string> GetLink(string id)
    {
        return await _http.GetAsync<string>($"links/{id}");
    }
}