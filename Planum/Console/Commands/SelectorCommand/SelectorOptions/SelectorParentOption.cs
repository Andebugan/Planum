using System;
using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorParentOption : SelectorBaseOption
    {
        public SelectorParentOption(CommandConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result, MatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
                throw new SelectorException("No arguments provided for option", OptionInfo);

            Guid id = Guid.Empty;
            if (!ValueParser.TryParse(ref id, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
                throw new SelectorException("Unable to parse id selector option", OptionInfo);

            IValueMatch<Guid> match = new ValueMatch<Guid>(id, args.Current);

            result.ParentFilter.AddMatch(match);
            return true;
        }
    }
}
