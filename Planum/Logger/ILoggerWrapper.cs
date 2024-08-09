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
        public void Log(LogLevel level = LogLevel.INFO, string message = "", [CallerLineNumber] int line = 0, [CallerMemberName] string? caller = null);
    }
}
