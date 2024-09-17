using System.Runtime.CompilerServices;

namespace Planum.Logger
{
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERR
    }

    public interface ILoggerWrapper
    {
        public void Log(string message = "", LogLevel level = LogLevel.INFO, [CallerLineNumber] int line = 0, [CallerMemberName] string? caller = null);
    }
}
