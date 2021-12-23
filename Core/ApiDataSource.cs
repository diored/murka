using DioRed.Murka.Core.Entities;

using System.Net.Http.Headers;
using System.Text.Json;

namespace DioRed.Murka.Core;

public class ApiDataSource : IDataSource
{
    private readonly Uri _apiServiceUri;
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new();
    
    static ApiDataSource()
    {
        _jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        _jsonSerializerOptions.AddDateOnlyConverters();
    }

    public ApiDataSource(Uri apiServiceUri)
    {
        _apiServiceUri = apiServiceUri;

        _httpClient = new HttpClient { BaseAddress = _apiServiceUri };
    }

    public DateTime GetServerTime()
    {
        return GetDateTime("serverTime");
    }

    public Event[] GetActiveEvents(DateTime dateTime)
    {
        return Get<Event[]>("activeEvents", dateTime.ToString("s"));
    }

    public Promocode[] GetActivePromocodes(DateTime dateTime)
    {
        return Get<Promocode[]>("activePromocodes", dateTime.ToString("s"));
    }

    public Daily GetDaily(DateTime dateTime)
    {
        return Get<Daily>("daily", dateTime.ToString("s"));
    }

    public IEnumerable<string> GetDayEvents(DateTime dateTime)
    {
        return Get<IEnumerable<string>>("dayEvents", dateTime.ToString("s"));
    }

    public string GetNorth(DateTime dateTime, NorthArmy army)
    {
        return Get<string>("north", dateTime.ToString("s"), army.ToString());
    }

    public string GetRandomGreeting()
    {
        return Get<string>("randomGreeting");
    }

    private TResult Get<TResult>(string method, params string[] args)
        where TResult: notnull
    {
        var stringResult = Task.Run(async () =>
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string path = string.Join("/", args.Prepend(method));

            return await _httpClient.GetStringAsync(path);
        }).GetAwaiter().GetResult();

        TResult? result = default;

        if (!string.IsNullOrEmpty(stringResult))
        {
            result = JsonSerializer.Deserialize<TResult>(stringResult, _jsonSerializerOptions);
        }

        if (result == null)
        {
            throw new InvalidOperationException("Empty result returned");
        }

        return result;
    }

    private DateTime GetDateTime(string method, params string[] args)
    {
        return DateTime.Parse(Get<string>(method, args));
    }
}
