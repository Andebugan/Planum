using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class DeadlineDurationOption : BaseOption 
    {
        public DeadlineDurationOption(CommandConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result)
        {
            MatchType matchType = MatchType.IGNORE;
            MatchFilterType filterType = MatchFilterType.SUBSTRING;

            if (!ExtractOptionParams(ref args, ref matchType, ref filterType))
                return false;

            if (!args.MoveNext())
                throw new SelectorException("No arguments provided for option", OptionInfo);

            TimeSpan duration = TimeSpan.Zero;
            if (!ValueParser.TryParse(ref duration, args.Current) && filterType != MatchFilterType.SUBSTRING)
                throw new SelectorException("Unable to parse id selector option", OptionInfo);

            IValueMatch<TimeSpan> match = new ValueMatch<TimeSpan>(duration, args.Current);

            result.DeadlineFilter.WarningFilter.AddMatch(match);
            return true;
        }
    }
}
