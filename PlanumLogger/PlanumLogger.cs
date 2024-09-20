using System.Runtime.CompilerServices;

namespace Planum.Logger
{
    public enum LogWhere
    {
        CONSOLE,
        FILE,
        CONSOLE_AND_FILE
    }

    public class PlanumLogger: ILoggerWrapper
    {
        public string LogFilePath { get; set; } = "log.txt";
        public LogWhere Where { get; set; } = LogWhere.FILE;
        public LogLevel Level { get; set; } = LogLevel.WARN; 

        Dictionary<LogLevel, string> LogLevelLabels = new Dictionary<LogLevel, string>() {
            {LogLevel.DEBUG, "[DEBUG]"},
            {LogLevel.INFO, "[INFO]"},
            {LogLevel.WARN, "[WARN]"},
            {LogLevel.ERR, "[ERR]"}
        };

        public PlanumLogger(LogLevel level = LogLevel.WARN, LogWhere where = LogWhere.FILE, string logFilePath = "log.txt", bool clearFile = false)
        {
            Level = level;
            Where = where;

            if (Where == LogWhere.FILE || Where == LogWhere.CONSOLE_AND_FILE)
            {
                if (!File.Exists(logFilePath))
                    File.Create(logFilePath).Close();
                else if (clearFile)
                    File.WriteAllText(logFilePath, "");
            }
        }

        public void Log(string message = "", LogLevel level = LogLevel.INFO, [CallerLineNumber] int line = 0, [CallerMemberName] string? caller = null)
        {
            if (level < Level)
                return;
            caller = caller is null ? "undefined" : caller;

            string logLine = $"{LogLevelLabels[level]} ({caller}:{line}) \"{message}\"";

            if (Where == LogWhere.FILE || Where == LogWhere.CONSOLE_AND_FILE)
                File.AppendAllLinesAsync(LogFilePath, new string[] { logLine }, System.Text.Encoding.UTF8);
            if (Where == LogWhere.CONSOLE || Where == LogWhere.CONSOLE_AND_FILE)
                System.Console.WriteLine(logLine);
        }
    }
}
