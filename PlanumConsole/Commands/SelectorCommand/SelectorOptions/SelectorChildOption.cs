using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorChildOption : SelectorBaseOption
    {
        public SelectorChildOption(ConsoleConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            Guid id = Guid.Empty;
            if (!ValueParser.TryParse(ref id, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse child selector option: {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            IValueMatch<Guid> match = new ValueMatch<Guid>(id, args.Current);

            result.ChildFilter.AddMatch(match);
            return true;
        }
    }
}
