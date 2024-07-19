using System;
using System.Collections.Generic;
using System.Globalization;

namespace Planum.Parser
{
    // how to print datetime: DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffffK");
    public static class ValueParser
    {
        public static string[] dateFormats = { "d.M.y", "d.M.yyyy", "d.M.yyyy", "d.M.y" };
        public static string[] timeFormats = { "H:m", "H:m" };

        public static Dictionary<string, DateTime> dateStringFormats = new Dictionary<string, DateTime>() {
            { "yesterday", DateTime.Today - TimeSpan.FromDays(1) },
            { "today", DateTime.Today },
            { "tomorrow", DateTime.Today + TimeSpan.FromDays(1) },
        };

        public static Dictionary<string, DayOfWeek> dayOfWeekStringFormats = new Dictionary<string, DayOfWeek>() {
            { "monday", DayOfWeek.Monday },
            { "tuesday", DayOfWeek.Tuesday},
            { "wednesday", DayOfWeek.Wednesday },
            { "thursday", DayOfWeek.Thursday},
            { "friday", DayOfWeek.Friday},
            { "saturday", DayOfWeek.Saturday},
            { "sunday", DayOfWeek.Sunday},
        };

        public static Dictionary<string, int> dateStringDayPrefixFormats = new Dictionary<string, int>() {
            { "next", -1 },
            { "previous", 1 }
        };

        public static Dictionary<string, bool> boolFormats = new Dictionary<string, bool>() {
            {"true", true},
            {"t", true},
            {"yes", true},
            {"y", true},
            {"false", false},
            {"f", false},
            {"no", false},
            {"n", false},
            {"1", true},
            {"0", false}
        };

        public static string[] TimeSpanFormats = { @"d\.h\:m", @"d\.h", @"h\:m" };

        public static bool TryParse(ref Guid value, string data) => Guid.TryParse(data, out value);
        public static bool TryParse(ref int value, string data) => int.TryParse(data, out value);
        public static bool TryParse(ref float value, string data) => float.TryParse(data, out value);

        public static bool TryParse(ref bool value, string data)
        {
            if (boolFormats.Keys.Contains(data))
            {
                value = boolFormats[data];
                return true;
            }
            return false;
        }

        public static bool TryParse(ref TimeSpan value, string data)
        {
            bool result = false;
            foreach (var formatStr in TimeSpanFormats)
            {
                result = TimeSpan.TryParseExact(data, formatStr, CultureInfo.InvariantCulture, TimeSpanStyles.None, out value);
                if (result)
                    break;
            }
            return result;
        }

        public static bool TryParse(ref TimeSpan value, ref int months, ref int years, string data)
        {
            bool result = false;
            data = data.Trim(' ', '\n');
            IEnumerator<string> dataEnumerator = (IEnumerator<string>)data.Split(' ').GetEnumerator();

            // years
            if (int.TryParse(dataEnumerator.Current, out years))
                if (!dataEnumerator.MoveNext())
                    return true;

            // months
            if (int.TryParse(dataEnumerator.Current, out months))

            // timespan
            foreach (var formatStr in TimeSpanFormats)
            {
                result = TimeSpan.TryParseExact(data, formatStr, CultureInfo.InvariantCulture, TimeSpanStyles.None, out value);
                if (result)
                    break;
            }
            return result;
        }

        static bool TryParseTime(ref DateTime value, IEnumerator<string> dataEnumerator)
        {
            // try parse time
            bool result = false;
            DateTime time = new DateTime();
            foreach (var formatStr in timeFormats)
            {
                result = DateTime.TryParseExact(dataEnumerator.Current, formatStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
                if (result)
                    break;
            }

            if (result)
                value = new DateTime(value.Year, value.Month, value.Day, time.Hour, time.Minute, time.Second);
            return result;
        }

        static bool TryParseDate(ref DateTime value, IEnumerator<string> dataEnumerator)
        {
            bool result = false;
            DateTime date = new DateTime();
            foreach (var formatStr in dateFormats)
            {
                result = DateTime.TryParseExact(dataEnumerator.Current, formatStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                if (result)
                    break;
            }

            if (!result)
            {
                foreach (var key in dateStringFormats.Keys)
                {
                    if (key.StartsWith(dataEnumerator.Current))
                    {
                        date = dateStringFormats[key];
                        break;
                    }
                }
            }

            if (result)
                value = new DateTime(date.Year, date.Month, date.Day, value.Hour, value.Minute, value.Second);
            return result;
        }

        static bool TryParseDateMoves(ref DateTime value, IEnumerator<string> dataEnumerator)
        {
            int directionPrefix = 0;
            foreach (var key in dateStringDayPrefixFormats.Keys)
            {
                if (key.StartsWith(dataEnumerator.Current))
                {
                    directionPrefix = dateStringDayPrefixFormats[key];
                    break;
                }
            }

            if (directionPrefix == 0)
                return false;
            dataEnumerator.MoveNext();

            var result = false;
            DayOfWeek dayOfWeek = DayOfWeek.Monday;
            foreach (var dayOfWeekFormat in dayOfWeekStringFormats.Keys)
            {
                if (dataEnumerator.Current.StartsWith(dayOfWeekFormat))
                {
                    result = true;
                    dayOfWeek = dayOfWeekStringFormats[dayOfWeekFormat];
                    break;
                }
            }
            
            if (!result)
                return false;

            while (value.DayOfWeek != dayOfWeek)
                value.AddDays(directionPrefix);
            return true;
        }

        public static bool TryParse(ref DateTime value, string data)
        {
            data = data.Trim(' ', '\n');
            IEnumerator<string> dataEnumerator = (IEnumerator<string>)data.Split(' ').GetEnumerator();

            value = DateTime.Today;

            if (data == string.Empty || !dataEnumerator.MoveNext())
                return true;

            bool result = TryParseTime(ref value, dataEnumerator);
            // try parse time
            if (result)
                if (!dataEnumerator.MoveNext())
                    return true;
                else
                    return false;

            // try parse date
            result = TryParseDate(ref value, dataEnumerator);
            if (result)
                if (!dataEnumerator.MoveNext())
                    return true;
                else
                    return false;

            // try parse date with prefix
            result = TryParseDateMoves(ref value, dataEnumerator);
            return result;
        }
    }
}
