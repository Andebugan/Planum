using Planum.Logger;
using Planum.Model.Entities;

namespace Planum.Model.Filters
{
    public class DeadlineFilter : IDeadlineFilter
    {
        protected ILoggerWrapper Logger;

        public IValueFilter<Guid> IdFilter { get; set; }
        public IValueFilter<bool> EnabledFilter { get; set; }
        public IValueFilter<TimeSpan> WarningFilter { get; set; }
        public IValueFilter<TimeSpan> DurationFilter { get; set; }
        public IValueFilter<bool> RepeatedFilter { get; set; }
        public IValueFilter<TimeSpan> RepeatSpanFilter { get; set; }
        public IValueFilter<int> RepeatYearsFilter { get; set; }
        public IValueFilter<int> RepeatMonthsFilter { get; set; }
        public IValueFilter<DateTime> DeadlineValueFilter { get; set; }
        public IValueFilter<Guid> DeadlineNextFilter { get; set; }

        public DeadlineFilter(
            ILoggerWrapper logger,
            IValueFilter<Guid>? idFilter = null,
            IValueFilter<bool>? enabledFilter = null,
            IValueFilter<TimeSpan>? warningFilter = null,
            IValueFilter<TimeSpan>? durationFilter = null,
            IValueFilter<bool>? repeatedFilter = null,
            IValueFilter<TimeSpan>? repeatSpanFilter = null,
            IValueFilter<int>? repeatYearsFilter = null,
            IValueFilter<int>? repeatMonthsFilter = null,
            IValueFilter<DateTime>? deadlineValueFilter = null,
            IValueFilter<Guid>? deadlineNextFilter = null
                )
        {
            Logger = logger;
            IdFilter = idFilter is null ? new ValueFilter<Guid>(Logger) : idFilter;
            EnabledFilter = enabledFilter is null ? new ValueFilter<bool>(Logger) : enabledFilter;
            WarningFilter = warningFilter is null ? new ValueFilter<TimeSpan>(Logger) : warningFilter;
            DurationFilter = durationFilter is null ? new ValueFilter<TimeSpan>(Logger) : durationFilter;
            RepeatedFilter = repeatedFilter is null ? new ValueFilter<bool>(Logger) : repeatedFilter;
            RepeatSpanFilter = repeatSpanFilter is null ? new ValueFilter<TimeSpan>(Logger) : repeatSpanFilter;
            RepeatYearsFilter = repeatYearsFilter is null ? new ValueFilter<int>(Logger) : repeatYearsFilter;
            RepeatMonthsFilter = repeatMonthsFilter is null ? new ValueFilter<int>(Logger) : repeatMonthsFilter;
            DeadlineValueFilter = deadlineValueFilter is null ? new ValueFilter<DateTime>(Logger) : deadlineValueFilter;
            DeadlineNextFilter = deadlineNextFilter is null ? new ValueFilter<Guid>(Logger) : deadlineNextFilter;
        }

        public IEnumerable<Deadline> Filter(IEnumerable<Deadline> deadlines)
        {
            return deadlines.Where(x =>
                        IdFilter.Match(x.Id) &&
                        EnabledFilter.Match(x.enabled) &&
                        WarningFilter.Match(x.warningTime) &&
                        DurationFilter.Match(x.duration) &&
                        RepeatedFilter.Match(x.repeated) &&
                        RepeatSpanFilter.Match(x.repeatSpan) &&
                        RepeatYearsFilter.Match(x.repeatYears) &&
                        RepeatMonthsFilter.Match(x.repeatMonths) &&
                        DeadlineValueFilter.Match(x.deadline) &&
                        (!x.next.Any() || DeadlineNextFilter.Filter(x.next).Any())
                    );
        }
    }
}
