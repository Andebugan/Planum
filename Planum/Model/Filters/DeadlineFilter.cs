using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Model.Entities;

namespace Planum.Model.Filters
{
    public class DeadlineFilter : IDeadlineFilter
    {
        public IValueFilter<bool> EnabledFilter { get; set; }
        public IValueFilter<TimeSpan> WarningFilter { get; set; }
        public IValueFilter<TimeSpan> DurationFilter { get; set; }
        public IValueFilter<bool> RepeatedFilter { get; set; }
        public IValueFilter<TimeSpan> RepeatSpanFilter { get; set; }
        public IValueFilter<int> RepeatYearsFilter { get; set; }
        public IValueFilter<int> RepeatMonthsFilter { get; set; }
        public IValueFilter<DateTime> DeadlineValueFilter { get; set; }

        public DeadlineFilter(
            IValueFilter<bool>? enabledFilter = null,
            IValueFilter<TimeSpan>? warningFilter = null,
            IValueFilter<TimeSpan>? durationFilter = null,
            IValueFilter<bool>? repeatedFilter = null,
            IValueFilter<TimeSpan>? repeatSpanFilter = null,
            IValueFilter<int>? repeatYearsFilter = null,
            IValueFilter<int>? repeatMonthsFilter = null,
            IValueFilter<DateTime>? deadlineValueFilter = null
                )
        {
            EnabledFilter = enabledFilter is null ? new ValueFilter<bool>() : enabledFilter;
            WarningFilter = warningFilter is null ? new ValueFilter<TimeSpan>() : warningFilter;
            DurationFilter = durationFilter is null ? new ValueFilter<TimeSpan>() : durationFilter;
            RepeatedFilter = repeatedFilter is null ? new ValueFilter<bool>() : repeatedFilter;
            RepeatSpanFilter = repeatSpanFilter is null ? new ValueFilter<TimeSpan>() : repeatSpanFilter;
            RepeatYearsFilter = repeatYearsFilter is null ? new ValueFilter<int>() : repeatYearsFilter;
            RepeatMonthsFilter = repeatMonthsFilter is null ? new ValueFilter<int>() : repeatMonthsFilter;
            DeadlineValueFilter = deadlineValueFilter is null ? new ValueFilter<DateTime>() : deadlineValueFilter;
        }

        public IEnumerable<Deadline> Filter(IEnumerable<Deadline> deadlines)
        {
            return deadlines.Where(x =>
                        EnabledFilter.Match(x.enabled) &&
                        WarningFilter.Match(x.warningTime) &&
                        DurationFilter.Match(x.duration) &&
                        RepeatedFilter.Match(x.repeated) &&
                        RepeatSpanFilter.Match(x.repeatSpan) &&
                        RepeatYearsFilter.Match(x.repeatYears) &&
                        RepeatMonthsFilter.Match(x.repeatMonths) &&
                        DeadlineValueFilter.Match(x.deadline)
                    );
        }
    }
}
