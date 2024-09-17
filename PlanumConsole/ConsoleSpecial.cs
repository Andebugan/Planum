namespace Planum.Console
{
    // reset all string: \x1b[0;0m
    public enum TextStyle
    {
        Normal = 0,
        Bold = 1,
        Dim = 2,
        Italic = 3,
        Underline = 4,
        Blinking = 5,
        Inverse = 7,
        Hidden = 8,
        Strikethrough = 9
    }

    public enum TextBackgroundColor
    {
        Black = 40,
        Red = 41,
        Green = 42,
        Yellow = 43,
        Blue = 44,
        Magenta = 45,
        Cyan = 46,
        White = 47,
        Default = 49,
        BrightBlack = 100,
        BrightRed = 101,
        BrightGreen = 102,
        BrightYellow = 103,
        BrightBlue = 104,
        BrightMagenta = 105,
        BrightCyan = 106,
        BrightWhite = 107,

    }

    public enum TextForegroundColor
    {
        Black = 30,
        Red = 31,
        Green = 32,
        Yellow = 33,
        Blue = 34,
        Magenta = 35,
        Cyan = 36,
        White = 37,
        Default = 39,
        BrightBlack = 90,
        BrightRed = 91,
        BrightGreen = 92,
        BrightYellow = 93,
        BrightBlue = 94,
        BrightMagenta = 95,
        BrightCyan = 96,
        BrightWhite = 97,
    } 

    public static class SpecialSymbols
    {
        public static char UpperLeft = '┌';
        public static char UpperRight = '┐';
        public static char UpperLeftRounded = '╭';
        public static char UpperRightRounded = '╮';
        public static char LowerLeft = '└';
        public static char LowerRight = '┘';
        public static char LowerLeftRounded = '╰';
        public static char LowerRightRounded = '╯';
        public static char VericalLine = '│';
        public static char HorizontalLine = '─';
        public static char Crossing = '┼';
        public static char CrossingLeft = '├';
        public static char CrossingRight = '┤';
    }

    public static class ConsoleInfoColors
    {
        public static TextForegroundColor Success { get; set; } = TextForegroundColor.BrightGreen;
        public static TextForegroundColor Info { get; set; } = TextForegroundColor.BrightCyan;
        public static TextForegroundColor Warning { get; set; } = TextForegroundColor.BrightYellow;
        public static TextForegroundColor Error { get; set; } = TextForegroundColor.BrightRed;
    }

    public static class ConsoleSpecial
    {
        public static string AddStyle(string text, TextStyle style = TextStyle.Normal, TextForegroundColor foregroundColor = TextForegroundColor.Default, TextBackgroundColor backgroundColor = TextBackgroundColor.Default)
        {
            return $"\x1b[{(int)style};{(int)foregroundColor};{(int)backgroundColor}m" + text + "\x1b[0;0m";
        }
    }
}
