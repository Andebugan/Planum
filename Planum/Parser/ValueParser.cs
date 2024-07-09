using System;
using System.Collections.Generic;
using System.Globalization;

namespace Planum.Parser
{
    // how to print datetime: DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffffK");
    public static class ValueParser
    {
        public static string[] dateTimeFormats = { "H:m d.M.y", "H:m d.M.yyyy", "d.M.yyyy", "d.M.y" };
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

        public static bool Parse(ref Guid value, string data) => Guid.TryParse(data, out value);
        public static bool Parse(ref int value, string data) => int.TryParse(data, out value);
        public static bool Parse(ref float value, string data) => float.TryParse(data, out value);

        public static bool Parse(ref bool value, string data)
        {
            if (boolFormats.Keys.Contains(data))
            {
                value = boolFormats[data];
                return true;
            }
            return false;
        }

        public static bool Parse(ref TimeSpan value, string data)
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

        public static bool Parse(ref DateTime value, string data)
        {
            bool result = false;
            foreach (var formatStr in dateTimeFormats)
            {
                result = DateTime.TryParseExact(data, formatStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
                if (result)
                    break;
            }
            return result;
        }
    }
}
