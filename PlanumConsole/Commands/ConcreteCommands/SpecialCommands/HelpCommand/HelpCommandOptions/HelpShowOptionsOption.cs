using Planum.Config;

namespace Planum.Console.Commands.Special
{
    public class HelpShowOptionsOption: BaseOption<HelpCommandSettings>
    {
        public HelpShowOptionsOption(OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref HelpCommandSettings result)
        {
            result.ShowOptions = true;
            return true;
        }
    }
}
