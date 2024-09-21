using Planum.Config;
using Planum.Logger;

namespace Planum.Console.Commands.Special
{
    public class HelpOptionLikeOption: BaseOption<HelpCommandSettings>
    {
        public HelpOptionLikeOption(ILoggerWrapper logger, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(logger, optionInfo, commandConfig) { }
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
