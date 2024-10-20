using Planum.Model.Filters;

namespace Planum.Console.Commands.Selector
{
    public enum MatchFilterType  
    {
        EQUAL,
        LESSER,
        LESSER_AND_EQUAL,
        GREATER,
        GREATER_AND_EQUAL,
        SUBSTRING
    }

    public static class SelectorOptionModifiersParser
    {
        static Dictionary<string, ValueMatchType> matchTypeParse = new Dictionary<string, ValueMatchType>()
        {
            { "+", ValueMatchType.AND },
            { "*", ValueMatchType.OR },
            { "!", ValueMatchType.NOT }
        };

        static Dictionary<string, MatchFilterType> matchFilterTypeParse = new Dictionary<string, MatchFilterType>()
        {
            { "<", MatchFilterType.LESSER },
            { "<=", MatchFilterType.LESSER_AND_EQUAL },
            { "==", MatchFilterType.EQUAL },
            { ">=", MatchFilterType.GREATER_AND_EQUAL },
            { ">", MatchFilterType.GREATER }
        };

        public static string ParseSelectorSettings(string selectorOptionName, out ValueMatchType matchType, out MatchFilterType filterType)
        {
            matchType = ValueMatchType.AND;
            filterType = MatchFilterType.SUBSTRING;

            var filter = matchFilterTypeParse.Keys.FirstOrDefault(x => selectorOptionName.EndsWith(x));
            if (filter is not null && filter != string.Empty && filter != "")
            {
                filterType = matchFilterTypeParse[filter];
                selectorOptionName = selectorOptionName.Replace(filter, "");
            }

            var match = matchTypeParse.Keys.FirstOrDefault(x => selectorOptionName.EndsWith(x));
            if (match is not null && match != string.Empty && match != "")
            {
                matchType = matchTypeParse[match];
                selectorOptionName = selectorOptionName.Replace(match, "");
            }

            return selectorOptionName;
        }
    }
}
