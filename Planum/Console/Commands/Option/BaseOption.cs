using System.Collections.Generic;
using Planum.Config;

namespace Planum.Commands
{
    public abstract class BaseOption<T> : IOption
    {
        public OptionInfo OptionInfo { get; set; }
        CommandConfig CommandConfig { get; set; }

        protected BaseOption(OptionInfo optionInfo, CommandConfig commandConfig)
        {
            OptionInfo = optionInfo;
            CommandConfig = commandConfig;
        }

        public bool CheckMatch(string value) => value.Trim(' ') == CommandConfig.OptionPrefix + OptionInfo.Name;

        public abstract bool TryParseValue(ref IEnumerator<string> args, ref T result);
    }
}
