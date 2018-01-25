using System;
using System.Linq;

using NodaTime;
using NodaTime.TimeZones;

namespace DateLib
{
    public interface IDateTimeWrapper
    {
        DateTime GetCountryTime(string countryCode);
        DateTime GetCurrentUtcTime();
        string GetUtcDateAsString();
    }

    public class DateTimeWrapper : IDateTimeWrapper
    {
        private const string EUROPEAN_DATE_FORMAT = "dd/MM/yyyy";

        public DateTime GetCountryTime(string countryCode)
        {
            var utcDate = GetCurrentUtcTime();

            var countryTimeZone = TzdbDateTimeZoneSource.Default.ZoneLocations?.FirstOrDefault(x => x.CountryCode == countryCode.ToUpper());
            if (countryTimeZone != null)
            {
                var zone = DateTimeZoneProviders.Tzdb[countryTimeZone.ZoneId];
                var offset = zone.GetUtcOffset(Instant.FromDateTimeUtc(utcDate));

                return utcDate.AddSeconds(offset.Seconds);
            }

            return utcDate;
        }

        public DateTime GetCurrentUtcTime()
        {
            return DateTime.UtcNow;
        }

        public string GetUtcDateAsString()
        {
            return DateTime.UtcNow.ToString(EUROPEAN_DATE_FORMAT);
        }
    }
}
