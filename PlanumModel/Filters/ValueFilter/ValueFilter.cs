using System;
using System.Collections.Generic;
using System.Linq;

namespace Planum.Model.Filters
{
    public class ValueFilter<T>: IValueFilter<T> where T : IComparable
    {
        List<IValueMatch<T>> valueMatches = new List<IValueMatch<T>>();

        public IValueFilter<T> AddMatch(IValueMatch<T> match)
        {
            valueMatches.Add(match);
            return this;
        }

        public IEnumerable<T> Filter(IEnumerable<T> values) => values.Where(x => Match(x));
        public bool Match(T value) => valueMatches.Where(x => x.Check(value)).Any();
    }
}
