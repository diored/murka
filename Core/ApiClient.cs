using System.Collections;

using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core;

public class ApiClient : IDisposable
{
    private readonly SimpleHttpClient _http;
    private bool _disposedValue;

    public ApiClient(ApiSettings apiSettings)
    {
        _http = new SimpleHttpClient(apiSettings.Uri, apiSettings.AccessToken);
    }

    public async Task<ICollection<ChatInfo>> GetTelegramChats()
    {
        return await _http.GetAsync<ICollection<ChatInfo>>("chats/get/telegram");
    }

    public async Task AddChat(string id, string type, string title)
    {
        await _http.PostAsync("chats/add", new { id, type, title });
    }

    public async Task RemoveChat(string id, string type)
    {
        await _http.PostAsync("chats/remove", new { id, type });
    }

    public async Task<Daily> GetDaily(string date)
    {
        return await _http.GetAsync<Daily>("daily/get", new { date });
    }

    public async Task<byte[]> GetDailyCalendar()
    {
        return await _http.GetAsync<byte[]>("daily/calendar");
    }

    public async Task SetDailyMonth(int month, string dailies)
    {
        await _http.PostAsync("daily/setMonth", new { month, dailies });
    }

    public async Task<ICollection<DayEvent>> GetDayEvents(string date, string chatId)
    {
        return await _http.GetAsync<ICollection<DayEvent>>("dayevents/get", new { date, chatId });
    }

    public async Task AddDayEvent(string name, string occurrence, string time, string? chatId)
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

    public async Task Log(string level, string message, object? argument = null, Exception? exception = null)
    {
        string? argumentString = argument is IEnumerable enumerable
            ? string.Join(", ", enumerable)
            : argument?.ToString();

        string? exceptionString = exception is null
            ? null
            : exception.GetType().Name + " — " + exception.Message;

        await _http.PostAsync("logs/add", new { level, message, argument = argumentString, exception = exceptionString });
    }

    #region Dispose pattern
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _http?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
