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
            { "", MatchFilterType.SUBSTRING },
            { "<", MatchFilterType.LESSER },
            { "<=", MatchFilterType.LESSER_AND_EQUAL },
            { "=", MatchFilterType.EQUAL },
            { ">=", MatchFilterType.GREATER_AND_EQUAL },
            { ">", MatchFilterType.GREATER }
        };

        public static string ParseSelectorSettings(string selectorOptionName, out ValueMatchType matchType, out MatchFilterType filterType)
        {
            matchType = ValueMatchType.AND;
            foreach (var match in matchTypeParse.Keys)
            {
                if (selectorOptionName.EndsWith(match))
                {
                    matchType = matchTypeParse[match];
                    selectorOptionName = selectorOptionName.Remove(selectorOptionName.Length - match.Length, match.Length);
                    break;
                }
            }

            filterType = MatchFilterType.SUBSTRING;
            foreach (var filter in matchFilterTypeParse.Keys)
            {
                if (selectorOptionName.EndsWith(filter))
                {
                    filterType = matchFilterTypeParse[filter];
                    selectorOptionName = selectorOptionName.Remove(selectorOptionName.Length - filter.Length, filter.Length);
                    break;
                }
            }

            return selectorOptionName;
        }
    }
}
