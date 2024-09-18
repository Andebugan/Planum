using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineRepeatSpanOption : SelectorBaseOption 
    {
        public SelectorDeadlineRepeatSpanOption(ConsoleConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
                throw new SelectorException("No arguments provided for option", OptionInfo);

            TimeSpan repeatSpan = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref repeatSpan, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
                throw new SelectorException("Unable to parse id selector option", OptionInfo);

            IValueMatch<TimeSpan> match = new ValueMatch<TimeSpan>(repeatSpan, args.Current);

            result.DeadlineFilter.RepeatSpanFilter.AddMatch(match);

            return true;
        }
    }
}
