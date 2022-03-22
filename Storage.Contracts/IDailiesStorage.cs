using DioRed.Murka.Common;
using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Storage.Contracts;

public interface IDailiesStorage
{
    Daily Get(DateOnly date);
    void SetMonth(int month, string dailies);
}
