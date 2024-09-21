using Planum.Config;
using Planum.Model.Filters;

namespace Planum.Console.Commands.Selector
{
    public abstract class SelectorBaseOption : IOption
    {
        public OptionInfo OptionInfo { get; set; }
        protected ConsoleConfig CommandConfig { get; set; }

        public SelectorBaseOption(ConsoleConfig commandConfig, OptionInfo optionInfo)
        {
            CommandConfig = commandConfig;
            OptionInfo = optionInfo;
        }

        public string ExtractOptionParams(string selectorHeader)
        {
            ValueMatchType matchType = ValueMatchType.IGNORE;
            MatchFilterType filterType = MatchFilterType.SUBSTRING;

            return SelectorOptionModifiersParser.ParseSelectorSettings(selectorHeader, out matchType, out filterType);
        }

        public string ExtractOptionParams(string selectorHeader, out ValueMatchType matchType, out MatchFilterType filterType)
        {
            matchType = ValueMatchType.IGNORE;
            filterType = MatchFilterType.SUBSTRING;

            return SelectorOptionModifiersParser.ParseSelectorSettings(selectorHeader, out matchType, out filterType);
        }

        public bool CheckMatch(string value) => value.Trim(' ') == CommandConfig.OptionPrefix + OptionInfo.Name;
        public abstract bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType);
    }
}
