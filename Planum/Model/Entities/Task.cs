using System;
using System.Collections.Generic;
using System.Linq;

namespace Planum.Model.Entities
{
    public class Deadline
    {
        public bool enabled;
        public DateTime deadline;
        public TimeSpan warningTime;
        public TimeSpan duration;

        public bool repeated;
        public TimeSpan repeatSpan;
        public int repeatYears;
        public int repeatMonths;

        public Deadline(bool enabled = false,
                DateTime? deadline = null,
                TimeSpan? warningTime = null,
                TimeSpan? duration = null,
                bool repeated = false,
                TimeSpan? repeatSpan = null,
                int repeatYears = 0,
                int repeatMonths = 0)
        {
            this.enabled = enabled;
            this.deadline = deadline is null ? DateTime.Now : (DateTime)deadline;
            this.warningTime = warningTime is null ? TimeSpan.Zero : (TimeSpan)warningTime;

            this.duration = duration is null ? TimeSpan.Zero : (TimeSpan)duration;

            this.repeated = repeated;
            this.repeatSpan = repeatSpan is null ? TimeSpan.Zero : (TimeSpan)repeatSpan;
            this.repeatYears = repeatYears;
            this.repeatMonths = repeatMonths;
        }
    }

    public class Task
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IEnumerable<Deadline> Deadlines { get; set; } = new List<Deadline>();

        public IEnumerable<Guid> Children { get; set; } = new List<Guid>();

        public IEnumerable<Guid> Parents { get; set; } = new List<Guid>();

        public Task(Guid? id = null,
                string name = "",
                string description = "",
                IEnumerable<Deadline>? deadlines = null,
                IEnumerable<Guid>? children = null,
                IEnumerable<Guid>? parents = null)
        {
            Id = id is null ? new Guid() : (Guid)id;
            Name = name;
            Description = description;
            if (deadlines is not null)
                Deadlines = deadlines.ToList();
            if (children is not null)
                Children = children.ToList();
            if (parents is not null)
                Parents = parents.ToList();
        }
    }
}
