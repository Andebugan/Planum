using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineRepeatSpanOption : SelectorBaseOption
    {
        public SelectorDeadlineRepeatSpanOption(ConsoleConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            TimeSpan repeatSpan = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref repeatSpan, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse repeat span selector option: {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            IValueMatch<TimeSpan> match = new ValueMatch<TimeSpan>(repeatSpan, args.Current);

            result.DeadlineFilter.RepeatSpanFilter.AddMatch(match);

            return true;
        }
    }
}
