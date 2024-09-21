using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineValueOption : SelectorBaseOption
    {
        public SelectorDeadlineValueOption(ConsoleConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            DateTime deadline = DateTime.Now;
            if (!ValueParser.TryParse(ref deadline, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse deadline value selector option: {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            IValueMatch<DateTime> match = new ValueMatch<DateTime>(deadline, args.Current);

            result.DeadlineFilter.DeadlineValueFilter.AddMatch(match);
            return true;
        }
    }
}
