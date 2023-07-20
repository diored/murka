using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace DioRed.Murka.Core;

public class SimpleHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _disposedValue;
    private static readonly JsonSerializerOptions _options;

    static SimpleHttpClient()
    {
        _options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        _options.Converters.Add(new JsonStringEnumConverter());
    }

    public SimpleHttpClient(string baseUri, string? accessToken = null)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUri),
            DefaultRequestVersion = HttpVersion.Version20
        };

        if (!string.IsNullOrEmpty(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    public async Task<HttpContent> GetAsync(string uri, object? arguments = null)
    {
        string requestUri = BuildRequestUri(uri, arguments);

        var response = await _httpClient.GetAsync(requestUri);
        EnsureSuccessStatusCode(response);

        return response.Content;
    }

    public async Task<T> GetAsync<T>(string uri, object? arguments = null)
        where T : class
    {
        HttpContent content = await GetAsync(uri, arguments);

        return await ReadContent<T>(content);
    }

    public async Task<HttpContent> PostAsync(string uri, object? body = null)
    {
        var response = body is null
            ? await _httpClient.PostAsync(uri, null)
            : await _httpClient.PostAsJsonAsync(uri, body);

        EnsureSuccessStatusCode(response);

        return response.Content;
    }

    public async Task<T> PostAsync<T>(string uri, object? body = null)
        where T : class
    {
        HttpContent content = await PostAsync(uri, body);

        return await ReadContent<T>(content);
    }

    private static async Task<T> ReadContent<T>(HttpContent content)
        where T : class
    {
        if (typeof(T) == typeof(string))
        {
            return Cast(await content.ReadAsStringAsync());
        }

        if (typeof(T) == typeof(byte[]))
        {
            return Cast(await content.ReadAsByteArrayAsync());
        }

        if (typeof(T) == typeof(Stream))
        {
            return Cast(await content.ReadAsStreamAsync());
        }

        return Cast(await content.ReadFromJsonAsync<T>(_options));

        static T Cast(object? obj)
        {
            return obj as T
                ?? throw new HttpRequestException("Request succeed, but response body cannot be parsed in expected format");
        }
    }

    private static void EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed. Error code: {response.StatusCode}.");
        }
    }

    private static string BuildRequestUri(string uri, object? arguments)
    {
        return arguments is null
            ? uri
            : uri + BuildQueryString(arguments);
    }

    private static string BuildQueryString(object arguments)
    {
        StringBuilder queryString = new("?");

        PropertyInfo[] properties = arguments.GetType().GetProperties();
        foreach (PropertyInfo property in properties)
        {
            if (property != properties[0])
            {
                queryString.Append('&');
            }

            queryString.Append(HttpUtility.UrlEncode(property.Name));
            queryString.Append('=');
            queryString.Append(HttpUtility.UrlEncode(property.GetValue(arguments)?.ToString()));
        }

        return queryString.ToString();
    }

    #region Dispose pattern
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
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
