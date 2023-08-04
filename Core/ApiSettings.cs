namespace DioRed.Murka.Core;

internal class ApiSettings
{
    public required string Uri { get; init; }
    public required Func<string> GetAccessToken { get; init; }
}
