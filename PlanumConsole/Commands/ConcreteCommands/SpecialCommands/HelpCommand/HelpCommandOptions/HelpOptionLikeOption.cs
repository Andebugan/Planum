using Planum.Config;

namespace Planum.Console.Commands.Special
{
    public class HelpOptionLikeOption: BaseOption<HelpCommandSettings>
    {
        public HelpOptionLikeOption(OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref HelpCommandSettings result)
        {
            args.MoveNext();
            result.CommandOptionLikeString = args.Current;
            return true;
        }
    }
}
