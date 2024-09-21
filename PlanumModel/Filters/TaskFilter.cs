using Planum.Logger;
using Planum.Model.Entities;

namespace Planum.Model.Filters
{
    public class TaskFilter : ITaskFilter
    {
        public IValueFilter<Guid> IdFilter { get; set; }
        public IValueFilter<string> NameFilter { get; set; }
        public IValueFilter<string> TagFilter { get; set; }
        public IValueFilter<string> DescriptionFilter { get; set; }
        public IValueFilter<Guid> ParentFilter { get; set; }
        public IValueFilter<Guid> ChildFilter { get; set; }
        public IDeadlineFilter DeadlineFilter { get; set; }

        protected ILoggerWrapper Logger;

        public TaskFilter(
                ILoggerWrapper logger,
                IValueFilter<Guid>? idFilter = null,
                IValueFilter<string>? nameFilter = null,
                IValueFilter<string>? tagFilter = null,
                IValueFilter<string>? descriptionFilter = null,
                IValueFilter<Guid>? parentFilter = null,
                IValueFilter<Guid>? childFilter = null,
                IDeadlineFilter? deadlineFilter = null
                )
        {
            Logger = logger;
            IdFilter = idFilter is null ? new ValueFilter<Guid>(Logger) : idFilter;
            NameFilter = nameFilter is null ? new ValueFilter<string>(Logger) : nameFilter;
            TagFilter = tagFilter is null ? new ValueFilter<string>(Logger) : tagFilter;
            DescriptionFilter = descriptionFilter is null ? new ValueFilter<string>(Logger) : descriptionFilter;
            ParentFilter = parentFilter is null ? new ValueFilter<Guid>(Logger) : parentFilter;
            ChildFilter = childFilter is null ? new ValueFilter<Guid>(Logger) : childFilter;
            DeadlineFilter = deadlineFilter is null ? new DeadlineFilter(Logger) : deadlineFilter;
        }

        public IEnumerable<PlanumTask> Filter(IEnumerable<PlanumTask> tasks)
        {
            return tasks.Where(x =>
                    IdFilter.Match(x.Id) &&
                    NameFilter.Match(x.Name) &&
                    (!x.Tags.Any() || TagFilter.Filter(x.Tags).Any()) &&
                    DescriptionFilter.Match(x.Description) &&
                    (!x.Parents.Any() || ParentFilter.Filter(x.Parents).Any()) &&
                    (!x.Children.Any() || ChildFilter.Filter(x.Children).Any()) &&
                    (!x.Deadlines.Any() || DeadlineFilter.Filter(x.Deadlines).Any())
                    );
        }
    }
}
