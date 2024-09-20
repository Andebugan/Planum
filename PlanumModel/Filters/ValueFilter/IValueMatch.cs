namespace Planum.Model.Filters
{
    public enum ValueMatchType
    {
        AND,
        OR,
        NOT,
        IGNORE
    }

    public interface IValueMatch<T> where T : IComparable
    {
        public T Value { get; set; }

        public ValueMatchType Equal { get; set; }
        public ValueMatchType Lesser { get; set; }
        public ValueMatchType Greater { get; set; }

        public ValueMatchType ValueStrInCompared { get; set; }
        public ValueMatchType ComparedStrInValue { get; set; }

        public bool Check(T compared);
    }
}
