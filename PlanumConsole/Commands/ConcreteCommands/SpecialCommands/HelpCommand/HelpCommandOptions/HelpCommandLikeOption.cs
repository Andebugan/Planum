using Planum.Config;
using Planum.Logger;

namespace Planum.Console.Commands.Special
{
    public class HelpCommandLikeOption: BaseOption<HelpCommandSettings>
    {
        public HelpCommandLikeOption(ILoggerWrapper logger, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(logger, optionInfo, commandConfig) { }
        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref HelpCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            result.CommandNameLikeString = args.Current;
            return true;
        }
    }
}
