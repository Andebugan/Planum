using Planum.Config;

namespace Planum.Console.Commands.Special
{
    public class HelpOptionLikeOption: BaseOption<HelpCommandSettings>
    {
        public HelpOptionLikeOption(OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref HelpCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            result.CommandOptionLikeString = args.Current;
            return true;
        }
    }
}
