using System.Collections.Generic;
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
        static Dictionary<string, MatchType> matchTypeParse = new Dictionary<string, MatchType>()
        {
            { "+", MatchType.AND },
            { "*", MatchType.OR },
            { "!", MatchType.NOT }
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

        public static string ParseSelectorSettings(string selectorOptionName, out MatchType matchType, out MatchFilterType filterType)
        {
            matchType = MatchType.AND;
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
