using Planum.Config;
using Planum.Model.Filters;

namespace Planum.Console.Commands.Selector;

public class SelectorNameOption : SelectorBaseOption
{
    public SelectorNameOption(ConsoleConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

    public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)
    {
        if (!args.MoveNext())
        {
            lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
            return false;
        }

        IValueMatch<string> match = new ValueMatch<string>(args.Current, args.Current);

        result.NameFilter.AddMatch(match);
        return true;
    }
}
