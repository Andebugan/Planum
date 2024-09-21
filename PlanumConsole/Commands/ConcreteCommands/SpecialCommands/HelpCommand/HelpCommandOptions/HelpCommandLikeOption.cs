using Planum.Config;

namespace Planum.Console.Commands.Special
{
    public class HelpCommandLikeOption: BaseOption<HelpCommandSettings>
    {
        public HelpCommandLikeOption(OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref HelpCommandSettings result)
        {
            if (!args.MoveNext())
            {
                return false;
            }
            result.CommandNameLikeString = args.Current;
            return true;
        }
    }
}
