using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core;

public class ApiClient
{
    private readonly ApiSettings _apiSettings;

    public ApiClient(ApiSettings apiSettings)
    {
        _apiSettings = apiSettings;
    }

    public async Task<ICollection<ChatInfo>> GetTelegramChats()
    {
        using var http = CreateHttpClient();

        return await http.GetAsync<ICollection<ChatInfo>>("chats/get/telegram");
    }

    public async Task AddChat(string id, string type, string title)
    {
        using var http = CreateHttpClient();

        await http.PostAsync("chats/add", new { id, type, title });
    }

    public async Task RemoveChat(string id, string type)
    {
        using var http = CreateHttpClient();

        await http.PostAsync("chats/remove", new { id, type });
    }

    public async Task<Daily> GetDaily(string date)
    {
        using var http = CreateHttpClient();

        return await http.GetAsync<Daily>("daily/get", new { date });
    }

    public async Task<byte[]> GetDailyCalendar()
    {
        using var http = CreateHttpClient();

        return await http.GetAsync<byte[]>("daily/calendar");
    }

    public async Task SetDailyMonth(int month, string dailies)
    {
        using var http = CreateHttpClient();

        await http.PostAsync("daily/setMonth", new { month, dailies });
    }

    public async Task<ICollection<DayEvent>> GetDayEvents(string date, string chatId)
    {
        using var http = CreateHttpClient();

        return await http.GetAsync<ICollection<DayEvent>>("dayevents/get", new { date, chatId });
    }

    public async Task AddDayEvent(string name, string occurrence, string time, string? chatId)
    {
        using var http = CreateHttpClient();

        await http.PostAsync("dayevents/add", new { name, occurrence, time, chatId });
    }

    public async Task<ICollection<Event>> GetActiveEvents()
    {
        using var http = CreateHttpClient();

        return await http.GetAsync<ICollection<Event>>("events/active");
    }

    public async Task AddEvent(string name, string? validFrom, string? validTo)
    {
        using var http = CreateHttpClient();

        await http.PostAsync("events/add", new { name, validFrom, validTo });
    }

    public async Task CleanupEvents()
    {
        using var http = CreateHttpClient();

        await http.PostAsync("events/cleanup");
    }

    public async Task<string> GetRandomGreeting()
    {
        using var http = CreateHttpClient();

        return await http.GetAsync<string>("greeting");
    }

    public async Task<Northlands> GetNorthlands(string date)
    {
        using var http = CreateHttpClient();

        return await http.GetAsync<Northlands>("north/get", new { date });
    }

    public async Task<ICollection<Promocode>> GetActivePromocodes()
    {
        using var http = CreateHttpClient();

        return await http.GetAsync<ICollection<Promocode>>("promocodes/active");
    }

    public async Task AddPromocode(string code, string? validFrom, string? validTo, string content)
    {
        using var http = CreateHttpClient();

        await http.PostAsync("promocodes/add", new { code, validFrom, validTo, content });
    }

    public async Task UpdatePromocode(string code, string? validFrom, string? validTo, string content)
    {
        using var http = CreateHttpClient();

        await http.PostAsync("promocodes/update", new { code, validFrom, validTo, content });
    }

    public async Task RemovePromocode(string code)
    {
        using var http = CreateHttpClient();

        await http.PostAsync("promocodes/remove", new { code });
    }

    public async Task CleanupPromocodes()
    {
        using var http = CreateHttpClient();

        await http.PostAsync("promocodes/cleanup");
    }

    public async Task<string> GetLink(string id)
    {
        using var http = CreateHttpClient();

        return await http.GetAsync<string>($"links/{id}");
    }

    private SimpleHttpClient CreateHttpClient()
    {
        return new SimpleHttpClient(_apiSettings.Uri, _apiSettings.GetAccessToken());
    }
}