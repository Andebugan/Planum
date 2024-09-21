using Planum.Config;
using Planum.Logger;
using Planum.Model.Filters;

namespace Planum.Console.Commands.Selector;

public class SelectorTagOption : SelectorBaseOption
{
    public SelectorTagOption(ILoggerWrapper logger, ConsoleConfig commandConfig, OptionInfo optionInfo) : base(logger, commandConfig, optionInfo) { }

    public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)

    {
        if (!args.MoveNext())
        {
            lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
            return false;
        }

        IValueMatch<string> match = new ValueMatch<string>(args.Current, args.Current);

        result.TagFilter.AddMatch(match);
        args.MoveNext();
        return true;
    }
}
