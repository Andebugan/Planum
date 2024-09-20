using Planum.Model.Entities;

namespace Planum.Model.Filters
{
    public interface ITaskFilter
    {
        public IValueFilter<Guid> IdFilter { get; set; }
        public IValueFilter<string> NameFilter { get; set; }
        public IValueFilter<string> DescriptionFilter { get; set; }
        public IValueFilter<Guid> ParentFilter { get; set; }
        public IValueFilter<Guid> ChildFilter { get; set; }
        public IDeadlineFilter DeadlineFilter { get; set; }

        public IEnumerable<PlanumTask> Filter(IEnumerable<PlanumTask> tasks);
    }
}
