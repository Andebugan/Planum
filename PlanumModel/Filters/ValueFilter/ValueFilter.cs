using Planum.Logger;

namespace Planum.Model.Filters
{
    public class ValueFilter<T>: IValueFilter<T> where T : IComparable
    {
        protected ILoggerWrapper Logger;

        public ValueFilter(ILoggerWrapper logger)
        {
            Logger = logger;
        }

        List<IValueMatch<T>> valueMatches = new List<IValueMatch<T>>();

        public IValueFilter<T> AddMatch(IValueMatch<T> match)
        {
            valueMatches.Add(match);
            return this;
        }

        public IEnumerable<T> Filter(IEnumerable<T> values) => values.Where(x => Match(x));
        public bool Match(T value)
        {
            if (!valueMatches.Any())
                return true;
            return valueMatches.Where(x => x.Check(value)).Any();
        }

        public override int GetHashCode()
        {
            return valueMatches.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
