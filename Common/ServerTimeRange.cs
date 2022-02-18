namespace DioRed.Murka.Common;

public struct ServerTimeRange
{
    public ServerTimeRange(ServerTime? from, ServerTime? to)
    {
        From = from;
        To = to;
    }

    public ServerTime? From { get; set; }
    public ServerTime? To { get; set; }

    public bool Contains(ServerTime serverTime)
    {
        if (From.HasValue)
        {
            if (From.Value.Date > serverTime.Date)
            {
                return false;
            }

            if (From.Value.Date == serverTime.Date &&
                From.Value.Time.HasValue &&
                From.Value.Time > serverTime.Time)
            {
                return false;
            }
        }

        if (To.HasValue)
        {
            if (To.Value.Date < serverTime.Date)
            {
                return false;
            }

            if (To.Value.Date == serverTime.Date &&
                To.Value.Time.HasValue &&
                To.Value.Time < serverTime.Time)
            {
                return false;
            }
        }

        return true;
    }
}
