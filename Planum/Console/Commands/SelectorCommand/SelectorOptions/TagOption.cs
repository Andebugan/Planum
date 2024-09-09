using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Filters;

namespace Planum.Console.Commands.Selector;

public class TagOption : BaseOption
{
    public TagOption(CommandConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

    public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result)
    {
        MatchType matchType = MatchType.IGNORE;
        MatchFilterType filterType = MatchFilterType.SUBSTRING;

        if (!ExtractOptionParams(ref args, ref matchType, ref filterType))
            return false;

        if (!args.MoveNext())
            throw new SelectorException("No arguments provided for option", OptionInfo);

        IValueMatch<string> match = new ValueMatch<string>(args.Current, args.Current);

        result.TagFilter.AddMatch(match);
        return true;
    }
}
