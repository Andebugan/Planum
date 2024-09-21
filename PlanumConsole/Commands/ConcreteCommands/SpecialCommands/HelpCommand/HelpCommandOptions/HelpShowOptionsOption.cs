using Planum.Config;
using Planum.Logger;

namespace Planum.Console.Commands.Special
{
    public class HelpShowOptionsOption: BaseOption<HelpCommandSettings>
    {
        public HelpShowOptionsOption(ILoggerWrapper logger, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(logger, optionInfo, commandConfig) { }
        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref HelpCommandSettings result)
        {
            result.ShowOptions = true;
            return true;
        }
    }
}
