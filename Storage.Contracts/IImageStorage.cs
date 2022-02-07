namespace DioRed.Murka.Storage.Contracts;

public interface IImageStorage
{
    BinaryData Get(string name);
}
