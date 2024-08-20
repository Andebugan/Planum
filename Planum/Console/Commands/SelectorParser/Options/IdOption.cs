using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Commands.Selector
{
    public class SelectorIdOption : SelectorBaseOption
    {
        public SelectorIdOption(CommandConfig commandConfig) : base(commandConfig)
        {
            OptionInfo = new OptionInfo("s", "select task via id or name", "s[match type][logic operator] {value (name or id, full or partial}");
        }

        public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result)
        {
            MatchType matchType = MatchType.IGNORE;
            MatchFilterType filterType = MatchFilterType.SUBSTRING;

            if (!ExtractOptionParams(ref args, ref matchType, ref filterType))
                return false;

            if (!args.MoveNext())
                throw new SelectorException("No arguments provided for option", OptionInfo);

            Guid id = Guid.Empty;
            if (!ValueParser.TryParse(ref id, args.Current) && filterType != MatchFilterType.SUBSTRING)
                throw new SelectorException("Unable to parse id selector option", OptionInfo);

            IValueMatch<Guid> match = new ValueMatch<Guid>(id, args.Current);

            result.IdFilter.AddMatch(match);
            return true;
        }
    }
}
