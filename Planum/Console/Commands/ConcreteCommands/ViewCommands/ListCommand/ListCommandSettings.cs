using System;
using System.Collections.Generic;

namespace Planum.Console.Commands.View
{
    public enum ListSortBy
    {
        NAME,
        DESCRIPTION,
        TAGS,
        PARENTS,
        CHILDREN,
        DEADLINE_ENABLED,
        DEADLINE_VALUE,
        DEADLINE_WARNING,
        DEADLINE_DURATION,
        DEADLINE_REPEATED,
        DEADLINE_REPEAT,
        DEADLINE_NEXT
    }

    public enum ListGroupBy
    {
        TAGS,
        PARENTS,
        CHILDREN,
        OVERDUE,
        IN_PROGRESS,
        WARNING
    }

    public enum ListSortDirection
    {
        ASCENDING,
        DESCENDING
    }

    public class ListCommandSettings : ICommandSettings
    {
        public bool Id { get; set; } = true;
        public bool Name { get; set; } = true;
        public bool Description { get; set; } = false;
        public bool Tags { get; set; } = false;
        public bool Children { get; set; } = false;
        public bool Parents { get; set; } = false;

        public bool Deadlines { get; set; } = false;
        public bool DeadlineEnabled { get; set; } = false;
        public bool DeadlineValue { get; set; } = false;
        public bool DeadlineWarning { get; set; } = false;
        public bool DeadlineDuration { get; set; } = false;
        public bool DeadlineRepeated { get; set; } = false;
        public bool DeadlineRepeat { get; set; } = false;
        public bool DeadlineNext { get; set; } = false;

        public List<Tuple<ListSortBy, ListSortDirection>> SortBy { get; set; } = new List<Tuple<ListSortBy, ListSortDirection>>();
        public List<ListGroupBy> GroupBy { get; set; } = new List<ListGroupBy>();

        public void SetAll()
        {
            Id = true;
            Name = true;
            Description = true;
            Tags = true;
            Children = true;
            Parents = true;

            Deadlines = true;
            DeadlineEnabled = true;
            DeadlineValue = true;
            DeadlineWarning = true;
            DeadlineDuration = true;
            DeadlineRepeated = true;
            DeadlineRepeat = true;
            DeadlineNext = true;
        }

        public void SetDeadline()
        {
            Deadlines = true;
            DeadlineEnabled = true;
            DeadlineValue = true;
            DeadlineWarning = true;
            DeadlineDuration = true;
            DeadlineRepeated = true;
            DeadlineRepeat = true;
            DeadlineNext = true;
        }
    }
}
