using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Model.Entities;

namespace Planum.Model.Filters
{
    public class TaskFilter : ITaskFilter
    {
        public IValueFilter<Guid> IdFilter { get; set; }
        public IValueFilter<string> NameFilter { get; set; }
        public IValueFilter<string> DescriptionFilter { get; set; }
        public IValueFilter<Guid> ParentFilter { get; set; }
        public IValueFilter<Guid> ChildFilter { get; set; }
        public IDeadlineFilter DeadlineFilter { get; set; }

        public TaskFilter(
                IValueFilter<Guid>? idFilter = null,
                IValueFilter<string>? nameFilter = null,
                IValueFilter<string>? descriptionFilter = null,
                IValueFilter<Guid>? parentFilter = null,
                IValueFilter<Guid>? childFilter = null,
                IDeadlineFilter? deadlineFilter = null
                )
        {
            IdFilter = idFilter is null ? new ValueFilter<Guid>() : idFilter;
            NameFilter = nameFilter is null ? new ValueFilter<string>() : nameFilter;
            DescriptionFilter = descriptionFilter is null ? new ValueFilter<string>() : descriptionFilter;
            ParentFilter = parentFilter is null ? new ValueFilter<Guid>() : parentFilter;
            ChildFilter = childFilter is null ? new ValueFilter<Guid>() : childFilter;
            DeadlineFilter = deadlineFilter is null ? new DeadlineFilter() : deadlineFilter;
        }

        public IEnumerable<PlanumTask> Filter(IEnumerable<PlanumTask> tasks)
        {
            return tasks.Where(x =>
                    IdFilter.Match(x.Id) &&
                    NameFilter.Match(x.Name) &&
                    DescriptionFilter.Match(x.Description) &&
                    ParentFilter.Filter(x.Parents).Any() &&
                    ChildFilter.Filter(x.Children).Any() &&
                    DeadlineFilter.Filter(x.Deadlines).Any()
                    );
        }
    }
}
