using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core.Modules;

public interface INorthModule
{
    Northlands GetNorth(DateOnly date);
}