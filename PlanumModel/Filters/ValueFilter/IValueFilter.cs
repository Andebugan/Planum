namespace Planum.Model.Filters
{
    public interface IValueFilter<T> where T : IComparable 
    {
        public IValueFilter<T> AddMatch(IValueMatch<T> match);
        public bool Match(T value);
        public IEnumerable<T> Filter(IEnumerable<T> values);
    }
}
