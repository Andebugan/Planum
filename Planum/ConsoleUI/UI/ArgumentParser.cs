using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Planum.ConsoleUI.UI
{
    // how to print datetime: DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffffK");
    public static class ArgumentParser
    {
        public static string DateTimeFormat = "[H:m] d.m.y";
        public static string DateFormat = "dd.MM.yyyy";
        public static string[] dateTimeFormats = { "H:m d.M.y", "H:m d.M.yyyy", "d.M.yyyy", "d.M.y"};
        public static string TimeSpanFormat = @"d\.h\:m";
        public static string RepeatPeriodFormat = "[yyyy] [mm] [d.hh:mm]";
        public static string NewLineSymbol = "~";
        public static string CommandDelimeter = "--";
        public static string NameSeparator = "|";

        public static string GetSpecialSymbols()
        {
            return "new line symbol: " + NewLineSymbol + ", string separator: " + NameSeparator;
        }

        public static string GetDateTimeFormat()
        {
            return "[hh:mm] dd.MM.yyyy";
        }

        public static string GetTimeSpanFormat()
        {
            return TimeSpanFormat.Replace("\\", "");
        }

        public static bool Parse(ref int value, ref List<string> args, int? defaultVal = null)
        {
            if (args.Count == 0 || args[0].StartsWith(CommandDelimeter))
            {
                if (defaultVal != null)
                {
                    value = (int)defaultVal;
                    return true;
                }
                else
                    return false;
            }
            if (!int.TryParse(args[0], out value))
                return false;
            args.RemoveAt(0);
            return true;
        }

        public static bool Parse(ref List<int> value, ref List<string> args, List<int>? defaultVal = null)
        {
            if (args.Count == 0 || args[0].StartsWith(CommandDelimeter))
            {
                if (defaultVal != null)
                {
                    value = defaultVal;
                    return true;
                }
                else
                    return false;
            }
            while (args.Count > 0 && !args[0].StartsWith(CommandDelimeter))
            {
                int val;
                if (!int.TryParse(args[0], out val))
                {
                    string[] range = args[0].Split('-');
                    if (range.Length != 2)
                        return false;
                    int start;
                    int end;
                    if (!int.TryParse(range[0], out start))
                        return false;
                    if (!int.TryParse(range[1], out end))
                        return false;
                    if (start > end || start < 0 || end < 0)
                        return false;
                    for (int i = start; i <= end; i++)
                    {
                        value.Add(i);
                    }
                }
                else
                {
                    value.Add(val);
                }
                args.RemoveAt(0);
            }

            value = value.Distinct().ToList();
            if (value.Count == 0)
                return false;
            return true;
        }

        public static bool Parse(ref string value, ref List<string> args, string? defaultVal = null)
        {
            if (args.Count == 0 || args[0].StartsWith(CommandDelimeter))
            {
                if (defaultVal != null)
                {
                    value = defaultVal;
                    return true;
                }
                else
                    return false;
            }

            if (args[0].StartsWith(CommandDelimeter))
                return false;

            value = "";
            while (args.Count > 0 && !args[0].StartsWith(CommandDelimeter))
            {
                value += args[0].Replace(NewLineSymbol, "\n");
                if (!value.EndsWith("\n"))
                    value += " ";
                args.RemoveAt(0);
            }
            if (value.Length > 0 && value[value.Length - 1] == ' ')
                value = value.Remove(value.Length - 1);
            return true;
        }

        public static bool Parse(ref List<string> value, ref List<string> args, List<string>? defaultVal = null)
        {
            if (args.Count == 0 || args[0].StartsWith(CommandDelimeter))
            {
                if (defaultVal != null)
                {
                    value = defaultVal;
                    return true;
                }
                else
                    return false;
            }

            if (args[0].StartsWith(CommandDelimeter))
                return false;

            string temp = "";
            while (args.Count > 0 && !args[0].StartsWith(CommandDelimeter))
            {
                if (args[0] == NameSeparator)
                {
                    if (temp[temp.Length - 1] == ' ')
                        temp = temp.Remove(temp.Length - 1);
                    value.Add(temp);
                    temp = "";
                }
                else
                {
                    temp += args[0].Replace(NewLineSymbol, "\n");
                    if (!temp.EndsWith("\n"))
                        temp += " ";
                }
                args.RemoveAt(0);
            }
            if (temp != "")
            {
                if (temp[temp.Length - 1] == ' ')
                    temp = temp.Remove(temp.Length - 1);
                value.Add(temp);
            }
            return true;
        }

        public static bool Parse(ref DateTime value, ref List<string> args, DateTime? defaultVal = null)
        {
            if (args.Count == 0 || args[0].StartsWith(CommandDelimeter))
            {
                if (defaultVal != null)
                {
                    value = (DateTime)defaultVal;
                    return true;
                }
                else
                    return false;
            }

            if (args[0].StartsWith(CommandDelimeter))
                return false;

            string dateTime = "00:00 " + args[0];
            bool result = false;
            foreach (var formatStr in dateTimeFormats)
            {
                result = DateTime.TryParseExact(dateTime, formatStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
                if (result)
                    break;
            }

            if (result)
            {
                args.RemoveAt(0);
                return true;
            }
            if (args.Count <= 1)
                return false;

            dateTime = args[0] + " " + args[1];

            foreach (var formatStr in dateTimeFormats)
            {
                result = DateTime.TryParseExact(dateTime, formatStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
                if (result)
                    break;
            }

            if (result)
            {
                args.RemoveAt(0);
                args.RemoveAt(0);
            }
            return result;
        }

        public static bool Parse(ref List<DateTime> value, ref List<string> args, List<DateTime>? defaultVal = null)
        {
            if (args.Count == 0 || args[0].StartsWith(CommandDelimeter))
            {
                if (defaultVal != null)
                {
                    value = defaultVal;
                    return true;
                }
                else
                    return false;
            }

            if (args[0].StartsWith(CommandDelimeter))
                return false;

            bool result = true;
            while (args.Count > 0 && !args[0].StartsWith(CommandDelimeter))
            {
                DateTime val = DateTime.MinValue;
                result = Parse(ref val, ref args);
                if (!result)
                    return false;
                value.Add(val);
            }
            return true;
        }

        public static bool Parse(ref bool value, ref List<string> args, bool? defaultVal = null)
        {
            if (args.Count == 0 || args[0].StartsWith(CommandDelimeter))
            {
                if (defaultVal != null)
                {
                    value = (bool)defaultVal;
                    return true;
                }
                else
                    return false;
            }

            if (args[0].StartsWith(CommandDelimeter))
                return false;

            if (args[0] == "y")
            {
                value = true;
                args.RemoveAt(0);
                return true;
            }
            else if (args[0] == "n")
            {
                value = false;
                args.RemoveAt(0);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Parse(ref List<bool> value, ref List<string> args, List<bool>? defaultVal = null)
        {
            if (args.Count == 0 || args[0].StartsWith(CommandDelimeter))
            {
                if (defaultVal != null)
                {
                    value = defaultVal;
                    return true;
                }
                else
                    return false;
            }

            if (args[0].StartsWith(CommandDelimeter))
                return false;

            bool result = true;
            bool val = true;
            while (args.Count > 0 && !args[0].StartsWith(CommandDelimeter) && result)
            {
                result = Parse(ref val, ref args);
                if (!result)
                    return false;
                value.Add(val);
            }
            return true;
        }

        public static bool Parse(ref TimeSpan value, ref List<string> args, TimeSpan? defaultVal = null)
        {
            if (args.Count == 0 || args[0].StartsWith(CommandDelimeter))
            {
                if (defaultVal != null)
                {
                    value = (TimeSpan)defaultVal;
                    return true;
                }
                else
                    return false;
            }
            if (!TimeSpan.TryParseExact(args[0], TimeSpanFormat, CultureInfo.InvariantCulture, TimeSpanStyles.None, out value))
                return false;
            args.RemoveAt(0);
            return true;
        }

        public static bool Parse(ref List<TimeSpan> value, ref List<string> args, List<TimeSpan>? defaultVal = null)
        {
            if (args.Count == 0 || args[0].StartsWith(CommandDelimeter))
            {
                if (defaultVal != null)
                {
                    value = defaultVal;
                    return true;
                }
                else
                    return false;
            }

            if (args[0].StartsWith(CommandDelimeter))
                return false;

            while (args.Count > 0 && !args[0].StartsWith(CommandDelimeter))
            {
                TimeSpan val;
                if (!TimeSpan.TryParseExact(args[0], TimeSpanFormat, CultureInfo.InvariantCulture, TimeSpanStyles.None, out val))
                    return false;
                value.Add(val);
                args.RemoveAt(0);
            }
            return true;
        }
    }
}
