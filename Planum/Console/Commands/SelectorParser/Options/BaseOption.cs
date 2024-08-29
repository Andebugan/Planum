using System.Collections.Generic;
using Planum.Config;
using Planum.Model.Filters;

namespace Planum.Commands.Selector
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

        public bool ExtractOptionParams(ref IEnumerator<string> args, ref MatchType matchType, ref MatchFilterType filterType)
        {
            matchType = MatchType.IGNORE;
            filterType = MatchFilterType.SUBSTRING;
            string selectorHeader = args.Current;

            SelectorOptionModifiersParser.ParseSelectorSettings(ref selectorHeader, out matchType, out filterType);
            return true;
        }

        public abstract bool TryParseValue(ref IEnumerator<string> args, ref TaskFilter result);
        public bool CheckMatch(string value) => !value.Trim(' ').StartsWith(CommandConfig.OptionPrefix + OptionInfo.Name);
    }
}
