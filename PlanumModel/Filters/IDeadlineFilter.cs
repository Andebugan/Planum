using Planum.Model.Entities;

namespace Planum.Model.Filters
{
    public interface IDeadlineFilter
    {
        IValueFilter<Guid> IdFilter { get; set; }
        IValueFilter<bool> EnabledFilter { get; set; }
        IValueFilter<DateTime> DeadlineValueFilter { get; set; }
        IValueFilter<TimeSpan> WarningFilter { get; set; }
        IValueFilter<TimeSpan> DurationFilter { get; set; }
        IValueFilter<bool> RepeatedFilter { get; set; }
        IValueFilter<RepeatSpan> RepeatSpanFilter { get; set; }

        IEnumerable<Deadline> Filter(IEnumerable<Deadline> deadlines);
    }
}
