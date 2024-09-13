using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineRepeatMonthsOption : SelectorBaseOption 
    {
        public SelectorDeadlineRepeatMonthsOption(CommandConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result, MatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
                throw new SelectorException("No arguments provided for option", OptionInfo);

            int months = 0;
            if (!ValueParser.TryParse(ref months, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
                throw new SelectorException("Unable to parse id selector option", OptionInfo);

            IValueMatch<int> match = new ValueMatch<int>(months, args.Current);

            result.DeadlineFilter.RepeatMonthsFilter.AddMatch(match);
            return true;
        }
    }
}
