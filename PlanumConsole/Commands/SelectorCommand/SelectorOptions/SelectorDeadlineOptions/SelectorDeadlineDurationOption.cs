using Planum.Config;
using Planum.Logger;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineDurationOption : SelectorBaseOption
    {
        public SelectorDeadlineDurationOption(ILoggerWrapper logger, ConsoleConfig commandConfig, OptionInfo optionInfo) : base(logger, commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)

        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            TimeSpan duration = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref duration, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse child selector option: {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            IValueMatch<TimeSpan> match = new ValueMatch<TimeSpan>(duration, args.Current);

            result.DeadlineFilter.WarningFilter.AddMatch(match);
            return true;
        }
    }
}
