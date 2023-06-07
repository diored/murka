namespace DioRed.Murka.Core;

public class ApiSettings
{
    public required string Uri { get; init; }
    public required Func<string> GetAccessToken { get; init; }
}
