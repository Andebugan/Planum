using Planum.Config;
using Planum.Logger;

namespace Planum.Console.Commands
{
    public abstract class BaseOption<T> : IOption
    {
        public OptionInfo OptionInfo { get; set; }
        protected ILoggerWrapper Logger { get; set; }
        protected ConsoleConfig ConsoleConfig { get; set; }

        protected BaseOption(ILoggerWrapper logger, OptionInfo optionInfo, ConsoleConfig commandConfig)
        {
            Logger = logger;
            OptionInfo = optionInfo;
            ConsoleConfig = commandConfig;
        }

        public virtual bool CheckMatch(string value) => value.Trim(' ') == ConsoleConfig.OptionPrefix + OptionInfo.Name;

        public abstract bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref T result);
    }
}
