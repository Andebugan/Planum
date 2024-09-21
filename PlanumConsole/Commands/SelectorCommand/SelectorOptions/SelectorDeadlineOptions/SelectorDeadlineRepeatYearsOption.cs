using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineRepeatYearsOption : SelectorBaseOption
    {
        public SelectorDeadlineRepeatYearsOption(ConsoleConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            int years = 0;
            if (!ValueParser.TryParse(ref years, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse repeat years selector option: {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            IValueMatch<int> match = new ValueMatch<int>(years, args.Current);

            result.DeadlineFilter.RepeatYearsFilter.AddMatch(match);
            return true;
        }
    }
}
