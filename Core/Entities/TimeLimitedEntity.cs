namespace DioRed.Murka.Core.Entities;

public record TimeLimitedEntity(ServerDateTime? ValidFrom, ServerDateTime? ValidTo)
{
    public bool ValidAt(ServerDateTime serverTime)
    {
        if (ValidFrom.HasValue)
        {
            if (ValidFrom.Value.Date > serverTime.Date)
            {
                return false;
            }

            if (ValidFrom.Value.Date == serverTime.Date &&
                ValidFrom.Value.Time.HasValue &&
                ValidFrom.Value.Time > serverTime.Time)
            {
                return false;
            }
        }

        if (ValidTo.HasValue)
        {
            if (ValidTo.Value.Date < serverTime.Date)
            {
                return false;
            }

            if (ValidTo.Value.Date == serverTime.Date &&
                ValidTo.Value.Time.HasValue &&
                ValidTo.Value.Time < serverTime.Time)
            {
                return false;
            }
        }

        return true;
    }
}