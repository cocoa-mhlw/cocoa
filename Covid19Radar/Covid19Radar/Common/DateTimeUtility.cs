using System;
namespace Covid19Radar.Common
{
    public interface IDateTimeUtility
    {
        DateTime UtcNow { get; }
    }
    public class DateTimeUtility : IDateTimeUtility
    {
        public static IDateTimeUtility Instance = new DateTimeUtility();

        public DateTimeUtility()
        {
        }

        public DateTime UtcNow => DateTime.UtcNow;
    }
}
