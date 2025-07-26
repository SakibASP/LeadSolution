namespace Common.Extentions;

public static class DateTimeExtention
{
    private static readonly TimeZoneInfo BdTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");

    public static DateTime ToBangladeshTime(this DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, BdTimeZone);
    }
}
