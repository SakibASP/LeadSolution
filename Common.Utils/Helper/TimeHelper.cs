namespace Common.Utils.Helper
{
    public static class TimeHelper
    {
        private static readonly TimeZoneInfo bdTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
        public static DateTime GetCurrentBangladeshTime() => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, bdTimeZone);
    }

}
