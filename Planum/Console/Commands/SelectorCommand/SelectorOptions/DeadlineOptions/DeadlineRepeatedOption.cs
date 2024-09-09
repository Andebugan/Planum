using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class DeadlineRepeatedOption : BaseOption 
    {
        public DeadlineRepeatedOption(CommandConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result)
        {
            MatchType matchType = MatchType.IGNORE;
            MatchFilterType filterType = MatchFilterType.SUBSTRING;

            if (!ExtractOptionParams(ref args, ref matchType, ref filterType))
                return false;

            if (!args.MoveNext())
                throw new SelectorException("No arguments provided for option", OptionInfo);

            bool enabled = true;
            if (!ValueParser.TryParse(ref enabled, args.Current) && filterType != MatchFilterType.SUBSTRING)
                throw new SelectorException("Unable to parse id selector option", OptionInfo);

            IValueMatch<bool> match = new ValueMatch<bool>(enabled, args.Current);

            result.DeadlineFilter.RepeatedFilter.AddMatch(match);
            return true;
        }
    }
}
