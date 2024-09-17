using System;

namespace Planum.Model.Filters
{
    public class ValueMatch<T> : IValueMatch<T> where T : IComparable
    {
        public T Value { get; set; }
        public string StringValue { get; set; }
        public MatchType Equal { get; set; }
        public MatchType Lesser { get; set; }
        public MatchType Greater { get; set; }

        public MatchType ValueStrInCompared { get; set; }
        public MatchType ComparedStrInValue { get; set; }

        public ValueMatch(T value,
                string stringValue,
                MatchType equal = MatchType.AND,
                MatchType lesser = MatchType.IGNORE,
                MatchType greater = MatchType.IGNORE,
                MatchType valueStrInCompared = MatchType.IGNORE,
                MatchType comparedStrInValue = MatchType.IGNORE)
        {
            Value = value;
            StringValue = stringValue;
            Equal = equal;
            Lesser = lesser;
            Greater = greater;
            ValueStrInCompared = valueStrInCompared;
            ComparedStrInValue = comparedStrInValue;
        }

        public bool Check(T compared)
        {
            bool result = false;

            result = CheckEqual(compared, result);
            result = CheckLesser(compared, result);
            result = CheckGreater(compared, result);
            result = CheckValueStrInCompared(compared, result);
            result = CheckComparedStrInValue(compared, result);

            return result;
        }

        // TODO: add string checks CheckEqualString, CheckLesserString and etc.
        public bool CheckEqual(T compared, bool agg = false)
        {
            if (Equal == MatchType.AND)
                return Value.Equals(compared);
            if (Equal == MatchType.OR)
                return agg || Value.Equals(compared);
            if (Equal == MatchType.NOT)
                return !Value.Equals(compared);
            return agg;
        }

        public bool CheckLesser(T compared, bool agg = false)
        {
            if (Lesser == MatchType.AND)
                return Value.CompareTo(compared) < 0;
            if (Lesser == MatchType.OR)
                return agg || (Value.CompareTo(compared) < 0);
            if (Lesser == MatchType.NOT)
                return !(Value.CompareTo(compared) < 0);
            return agg;
        }

        public bool CheckGreater(T compared, bool agg = false)
        {
            if (Lesser == MatchType.AND)
                return Value.CompareTo(compared) > 0;
            if (Lesser == MatchType.OR)
                return agg || (Value.CompareTo(compared) > 0);
            if (Lesser == MatchType.NOT)
                return !(Value.CompareTo(compared) > 0);
            return agg;
        }

        public bool CheckValueStrInCompared(T compared, bool agg = false)
        {
            var comparedStr = compared.ToString();
            var valueStr = Value.ToString();
            if (comparedStr is null || valueStr is null)
                return false;
            if (ValueStrInCompared == MatchType.AND)
                return comparedStr.IndexOf(valueStr) >= 0 ? true : false;
            if (ValueStrInCompared == MatchType.OR)
                return agg || comparedStr.IndexOf(valueStr) >= 0 ? true : false;
            if (ValueStrInCompared == MatchType.NOT)
                return comparedStr.IndexOf(valueStr) >= 0 ? false: true;
            return agg;
        }

        public bool CheckComparedStrInValue(T compared, bool agg = false)
        {
            var comparedStr = compared.ToString();
            var valueStr = Value.ToString();
            if (comparedStr is null || valueStr is null)
                return false;
            if (ValueStrInCompared == MatchType.AND)
                return valueStr.IndexOf(comparedStr) >= 0 ? true : false;
            if (ValueStrInCompared == MatchType.OR)
                return agg || valueStr.IndexOf(comparedStr) >= 0 ? true : false;
            if (ValueStrInCompared == MatchType.NOT)
                return valueStr.IndexOf(comparedStr) >= 0 ? false: true;
            return agg;
        }
    }
}
