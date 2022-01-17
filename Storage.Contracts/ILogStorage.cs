namespace DioRed.Murka.Storage.Contracts;

public interface ILogStorage
{
    void Log(string level, string message, object? argument = null, Exception? exception = null);
}
