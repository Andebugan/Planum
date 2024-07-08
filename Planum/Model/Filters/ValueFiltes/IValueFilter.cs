using System.Collections.Generic;

namespace Planum.Model.Filters
{
    public interface IValueFilter<T, IValueMatch>
    {
        public bool Match(T value);
        public IEnumerable<T> Filter(IEnumerable<T> values);
    }
}
