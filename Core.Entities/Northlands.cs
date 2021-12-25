namespace DioRed.Murka.Core.Entities;

public record Northlands(string North, string Gods)
{
    public string For(Army army) => army switch
    {
        Army.Gods => Gods,
        Army.North => North,
        _ => throw new ArgumentOutOfRangeException()
    };
}