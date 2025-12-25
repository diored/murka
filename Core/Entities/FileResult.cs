namespace DioRed.Murka.Infrastructure.Models;

public class FileResult
{
    public required byte[] Content { get; init; }
    public required string ContentType { get; init; }
}