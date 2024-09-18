using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineDurationOption : SelectorBaseOption 
    {
        public SelectorDeadlineDurationOption(ConsoleConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
                throw new SelectorException("No arguments provided for option", OptionInfo);

            TimeSpan duration = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref duration, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
                throw new SelectorException("Unable to parse id selector option", OptionInfo);

            IValueMatch<TimeSpan> match = new ValueMatch<TimeSpan>(duration, args.Current);

            result.DeadlineFilter.WarningFilter.AddMatch(match);
            return true;
        }
    }
}
