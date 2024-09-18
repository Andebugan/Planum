using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineEnabledOption : SelectorBaseOption 
    {
        public SelectorDeadlineEnabledOption(ConsoleConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
                throw new SelectorException("No arguments provided for option", OptionInfo);

            bool enabled = true;
            if (!ValueParser.TryParse(ref enabled, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
                throw new SelectorException("Unable to parse id selector option", OptionInfo);

            IValueMatch<bool> match = new ValueMatch<bool>(enabled, args.Current);


            result.DeadlineFilter.EnabledFilter.AddMatch(match);
            return true;
        }
    }
}
