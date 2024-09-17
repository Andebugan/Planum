using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Filters;

namespace Planum.Console.Commands.Selector
{
    public abstract class SelectorBaseOption : IOption
    {
        public OptionInfo OptionInfo { get; set; }
        protected CommandConfig CommandConfig { get; set; }

        public SelectorBaseOption(CommandConfig commandConfig, OptionInfo optionInfo)
        {
            CommandConfig = commandConfig;
            OptionInfo = optionInfo;
        }

        public string ExtractOptionParams(string selectorHeader)
        {
            MatchType matchType = MatchType.IGNORE;
            MatchFilterType filterType = MatchFilterType.SUBSTRING;

            return SelectorOptionModifiersParser.ParseSelectorSettings(selectorHeader, out matchType, out filterType);
        }

        public string ExtractOptionParams(string selectorHeader, out MatchType matchType, out MatchFilterType filterType)
        {
            matchType = MatchType.IGNORE;
            filterType = MatchFilterType.SUBSTRING;

            return SelectorOptionModifiersParser.ParseSelectorSettings(selectorHeader, out matchType, out filterType);
        }

        public bool CheckMatch(string value) => value.Trim(' ') == CommandConfig.OptionPrefix + OptionInfo.Name;
        public abstract bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result, MatchType matchType, MatchFilterType matchFilterType);
    }
}
