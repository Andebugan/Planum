using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineValueOption : SelectorBaseOption 
    {
        public SelectorDeadlineValueOption(CommandConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result, MatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
                throw new SelectorException("No arguments provided for option", OptionInfo);

            DateTime deadline = DateTime.Now;
            if (!ValueParser.TryParse(ref deadline, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
                throw new SelectorException("Unable to parse id selector option", OptionInfo);

            IValueMatch<DateTime> match = new ValueMatch<DateTime>(deadline, args.Current);

            result.DeadlineFilter.DeadlineValueFilter.AddMatch(match);
            return true;
        }
    }
}
