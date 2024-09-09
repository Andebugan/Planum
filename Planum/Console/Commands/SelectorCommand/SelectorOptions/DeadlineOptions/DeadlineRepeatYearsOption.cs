using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class DeadlineRepeatYearsOption : BaseOption
    {
        public DeadlineRepeatYearsOption(CommandConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result)
        {
            MatchType matchType = MatchType.IGNORE;
            MatchFilterType filterType = MatchFilterType.SUBSTRING;

            if (!ExtractOptionParams(ref args, ref matchType, ref filterType))
                return false;

            if (!args.MoveNext())
                throw new SelectorException("No arguments provided for option", OptionInfo);

            int years = 0;
            if (!ValueParser.TryParse(ref years, args.Current) && filterType != MatchFilterType.SUBSTRING)
                throw new SelectorException("Unable to parse id selector option", OptionInfo);

            IValueMatch<int> match = new ValueMatch<int>(years, args.Current);

            result.DeadlineFilter.RepeatYearsFilter.AddMatch(match);
            return true;
        }
    }
}
