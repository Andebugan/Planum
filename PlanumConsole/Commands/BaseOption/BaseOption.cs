using Planum.Config;

namespace Planum.Console.Commands
{
    public abstract class BaseOption<T> : IOption
    {
        public OptionInfo OptionInfo { get; set; }
        protected ConsoleConfig CommandConfig { get; set; }

        protected BaseOption(OptionInfo optionInfo, ConsoleConfig commandConfig)
        {
            OptionInfo = optionInfo;
            CommandConfig = commandConfig;
        }

        public bool CheckMatch(string value) => value.Trim(' ') == CommandConfig.OptionPrefix + OptionInfo.Name;

        public abstract bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref T result);
    }
}
