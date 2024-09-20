using System.Globalization;

namespace Planum.Parser
{
    /// <summary>
    /// Tools for extracting value from string with respect for special cases
    /// </summary>
    public static class ValueParser
    {
        public static string[] dateFormats = { "d.M.y", "d.M.yyyy", "d.M.yyyy", "d.M" };
        public static string[] timeFormats = { "H:m" };

        public static Dictionary<string, DateTime> dateStringFormats = new Dictionary<string, DateTime>() {
            { "yesterday", DateTime.Today.AddDays(-1) },
            { "today", DateTime.Today },
            { "tomorrow", DateTime.Today.AddDays(1)  },
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
            { "next", 1 },
            { "previous", -1 }
        };

        public static Dictionary<string, bool> boolFormats = new Dictionary<string, bool>() {
            {"true", true},
            {"yes", true},
            {"false", false},
            {"no", false},
            {"1", true},
            {"0", false}
        };

        public static string[] TimeSpanFormats = { @"d\.h\:m", @"d\.h", @"h\:m" };

        public static bool TryParse(ref Guid value, string data) => Guid.TryParse(data, out value);
        public static bool TryParse(ref int value, string data)
        {
            if (data == string.Empty)
            {
                value = 0;
                return true;
            }
            return int.TryParse(data, out value);
        }
        public static bool TryParse(ref float value, string data)
        {
            if (data == string.Empty)
            {
                value = 0;
                return true;
            }
            return float.TryParse(data, out value);
        }

        public static bool TryParse(ref bool value, string data)
        {
            if (data == string.Empty)
                return false;
            var matches = boolFormats.Keys.Where(x => x.StartsWith(data));
            if (matches.Any())
            {
                value = boolFormats[matches.First()];
                return true;
            }
            return false;
        }

        public static bool TryParse(ref TimeSpan value, string data)
        {
            if (data == string.Empty)
                return true;
            foreach (var formatStr in TimeSpanFormats)
            {
                if (TimeSpan.TryParseExact(data, formatStr, CultureInfo.InvariantCulture, TimeSpanStyles.None, out value))
                    return true;
            }
            return false;
        }

        public static bool TryParse(ref TimeSpan timeSpan, ref int months, ref int years, string data)
        {
            data = data.Trim(' ', '\n');
            var split = data.Split(' ').AsEnumerable();
            IEnumerator<string> dataEnumerator = (IEnumerator<string>)split.GetEnumerator();
            dataEnumerator.MoveNext();
            
            var tmp_months = 0;
            var tmp_years = 0;

            // months
            if (int.TryParse(dataEnumerator.Current, out tmp_months))
            {
                months = tmp_months;
                if (!dataEnumerator.MoveNext())
                    return true;
            }

            // years
            if (int.TryParse(dataEnumerator.Current, out tmp_years))
            {
                months = tmp_years;
                years = tmp_months;
                if (!dataEnumerator.MoveNext())
                    return true;
            }

            // timespan
            return TryParse(ref timeSpan, dataEnumerator.Current);
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
                        result = true;
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
            if (!dataEnumerator.MoveNext())
                return false;

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

            value = value.AddDays(directionPrefix);
            while (value.DayOfWeek != dayOfWeek)
                value = value.AddDays(directionPrefix);
            return true;
        }

        public static bool TryParse(ref DateTime value, string data)
        {
            data = data.Trim(' ', '\n');
            var split = data.Split(' ').AsEnumerable();
            IEnumerator<string> dataEnumerator = (IEnumerator<string>)split.GetEnumerator();
            value = DateTime.Today;

            if (data == string.Empty || !dataEnumerator.MoveNext())
                return true;

            var result = false;
            // try parse time
            if (TryParseTime(ref value, dataEnumerator))
            {
                result = true;
                if (!dataEnumerator.MoveNext())
                    return true;
            }

            // try parse date
            if (TryParseDate(ref value, dataEnumerator))
            {
                result = true;
                if (!dataEnumerator.MoveNext())
                    return true;
            }

            // try parse date with prefix
            if (!TryParseDateMoves(ref value, dataEnumerator))
                return false;
            else
                result = true;
            return result;
        }
    }
}
