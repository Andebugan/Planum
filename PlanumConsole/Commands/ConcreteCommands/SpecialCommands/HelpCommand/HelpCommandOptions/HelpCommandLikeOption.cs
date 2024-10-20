using Planum.Config;
using Planum.Logger;

namespace Planum.Console.Commands.Special
{
    public class HelpCommandLikeOption: BaseOption<HelpCommandSettings>
    {
        public HelpCommandLikeOption(ILoggerWrapper logger, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(logger, optionInfo, commandConfig) { }

        public override bool CheckMatch(string value) => base.CheckMatch(value) || !value.Trim(' ').StartsWith(ConsoleConfig.OptionPrefix);

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref HelpCommandSettings result)
        {
            if (args.Current != null && args.Current.Trim() != OptionInfo.Name)
                result.CommandNameLikeString = args.Current;
            else if (args.MoveNext() && args.Current != null)
                result.CommandNameLikeString = args.Current;
            else if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            return true;
        }
    }
}
