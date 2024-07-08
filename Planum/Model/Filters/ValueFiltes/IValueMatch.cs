using System;

namespace Planum.Model.Filters
{
    public enum MatchType
    {
        AND,
        OR,
        NOT,
        IGNORE
    }

    public interface IValueMatch<T> where T : IComparable
    {
        public T Value { get; set; }

        public MatchType Equal { get; set; }
        public MatchType Lesser { get; set; }
        public MatchType Greater { get; set; }

        public MatchType ValueStrInCompared { get; set; }
        public MatchType ComparedStrInValue { get; set; }

        public bool Check(T compared);
    }
}
