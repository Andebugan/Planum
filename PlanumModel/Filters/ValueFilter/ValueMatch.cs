namespace Planum.Model.Filters
{
    public class ValueMatch<T> : IValueMatch<T> where T : IComparable
    {
        public T Value { get; set; }
        public string StringValue { get; set; }
        public ValueMatchType Equal { get; set; }
        public ValueMatchType Lesser { get; set; }
        public ValueMatchType Greater { get; set; }

        public ValueMatchType ValueStrInCompared { get; set; }
        public ValueMatchType ComparedStrInValue { get; set; }

        public ValueMatch(T value,
                string stringValue,
                ValueMatchType equal = ValueMatchType.AND,
                ValueMatchType lesser = ValueMatchType.IGNORE,
                ValueMatchType greater = ValueMatchType.IGNORE,
                ValueMatchType valueStrInCompared = ValueMatchType.IGNORE,
                ValueMatchType comparedStrInValue = ValueMatchType.IGNORE)
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

        public bool CheckEqual(T compared, bool agg = false)
        {
            if (Equal == ValueMatchType.AND)
                return Value.Equals(compared);
            if (Equal == ValueMatchType.OR)
                return agg || Value.Equals(compared);
            if (Equal == ValueMatchType.NOT)
                return !Value.Equals(compared);
            return agg;
        }

        /// <summary>
        /// Checks if value is lesser than compared
        /// </summary>
        public bool CheckLesser(T compared, bool agg = false)
        {
            if (Lesser == ValueMatchType.AND)
                return Value.CompareTo(compared) < 0;
            if (Lesser == ValueMatchType.OR)
                return agg || (Value.CompareTo(compared) < 0);
            if (Lesser == ValueMatchType.NOT)
                return !(Value.CompareTo(compared) < 0);
            return agg;
        }

        /// <summary>
        /// Checks if value is greater than compared
        /// </summary>
        public bool CheckGreater(T compared, bool agg = false)
        {
            if (Lesser == ValueMatchType.AND)
                return Value.CompareTo(compared) > 0;
            if (Lesser == ValueMatchType.OR)
                return agg || (Value.CompareTo(compared) > 0);
            if (Lesser == ValueMatchType.NOT)
                return !(Value.CompareTo(compared) > 0);
            return agg;
        }

        public bool CheckValueStrInCompared(T compared, bool agg = false)
        {
            var comparedStr = compared.ToString();
            var valueStr = Value.ToString();
            if (comparedStr is null || valueStr is null)
                return false;
            if (ValueStrInCompared == ValueMatchType.AND)
                return comparedStr.IndexOf(valueStr) >= 0 ? true : false;
            if (ValueStrInCompared == ValueMatchType.OR)
                return agg || comparedStr.IndexOf(valueStr) >= 0 ? true : false;
            if (ValueStrInCompared == ValueMatchType.NOT)
                return comparedStr.IndexOf(valueStr) >= 0 ? false: true;
            return agg;
        }

        public bool CheckComparedStrInValue(T compared, bool agg = false)
        {
            var comparedStr = compared.ToString();
            var valueStr = Value.ToString();
            if (comparedStr is null || valueStr is null)
                return false;
            if (ValueStrInCompared == ValueMatchType.AND)
                return valueStr.IndexOf(comparedStr) >= 0 ? true : false;
            if (ValueStrInCompared == ValueMatchType.OR)
                return agg || valueStr.IndexOf(comparedStr) >= 0 ? true : false;
            if (ValueStrInCompared == ValueMatchType.NOT)
                return valueStr.IndexOf(comparedStr) >= 0 ? false: true;
            return agg;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value.GetHashCode(),
                StringValue.GetHashCode(),
                Equal.GetHashCode(),
                Lesser.GetHashCode(),
                Greater.GetHashCode(),
                ValueStrInCompared.GetHashCode(),
                ComparedStrInValue.GetHashCode()
                );
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
