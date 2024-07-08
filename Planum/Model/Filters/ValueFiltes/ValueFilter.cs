using System.Collections.Generic;
using System.Linq;

namespace Planum.Model.Filters
{
    public class ValueFilter<T, IValueMatch>: IValueFilter<T, IValueMatch>
    {
        HashSet<IValueMatch<T>> matchValues = new HashSet<IValueMatch<T>>();

        public IEnumerable<T> Filter(IEnumerable<T> values)
        {
            return values.Where(x => Match(x));
        }

        public bool Match(T value)
        {
            foreach (var match in matchValues)
            {
            }
        }
    }
}
