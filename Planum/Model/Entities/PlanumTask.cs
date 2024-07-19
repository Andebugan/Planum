#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Config;

namespace Planum.Model.Entities
{
    public class Deadline
    {
        public Guid Id { get; set; }
        public bool enabled;
        public DateTime deadline;
        public TimeSpan warningTime;
        public TimeSpan duration;

        public bool repeated;
        public TimeSpan repeatSpan;
        public int repeatYears;
        public int repeatMonths;

        public HashSet<Guid> next;

        public Deadline(bool enabled = false,
                DateTime? deadline = null,
                TimeSpan? warningTime = null,
                TimeSpan? duration = null,
                bool repeated = false,
                TimeSpan? repeatSpan = null,
                int repeatYears = 0,
                int repeatMonths = 0,
                HashSet<Guid>? next = null)
        {
            Id = Guid.NewGuid();
            this.enabled = enabled;
            this.deadline = deadline is null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0) : (DateTime)deadline;
            this.warningTime = warningTime is null ? TimeSpan.Zero : (TimeSpan)warningTime;

            this.duration = duration is null ? TimeSpan.Zero : (TimeSpan)duration;

            this.repeated = repeated;
            this.repeatSpan = repeatSpan is null ? TimeSpan.Zero : (TimeSpan)repeatSpan;
            this.repeatYears = repeatYears;
            this.repeatMonths = repeatMonths;
            this.next = next is null ? new HashSet<Guid>() : next;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            return Equals((Deadline)obj);
        }

        public bool Warning() => (deadline - duration - warningTime) < DateTime.Now; 
        public bool InProgress() => (deadline - duration) < DateTime.Now; 
        public bool Overdue() => DateTime.Now > deadline;

        public bool Equals(Deadline compared)
        {
            return enabled == compared.enabled &&
                deadline.Year == compared.deadline.Year &&
                deadline.Month == compared.deadline.Month &&
                deadline.Day == compared.deadline.Day &&
                deadline.Hour == compared.deadline.Hour &&
                deadline.Minute == compared.deadline.Minute &&
                TimeSpan.Equals(warningTime, compared.warningTime) &&
                TimeSpan.Equals(duration, compared.duration) &&
                repeated == compared.repeated &&
                TimeSpan.Equals(repeatSpan, compared.repeatSpan) &&
                repeatYears == compared.repeatYears &&
                repeatMonths == compared.repeatMonths &&
                next.SequenceEqual(compared.next);
        }

        public override int GetHashCode()
        {
            int hash = deadline.GetHashCode();
            hash ^= deadline.Year.GetHashCode();
            hash ^= deadline.Month.GetHashCode();
            hash ^= deadline.Day.GetHashCode();
            hash ^= deadline.Hour.GetHashCode();
            hash ^= deadline.Minute.GetHashCode();
            hash ^= warningTime.GetHashCode();
            hash ^= duration.GetHashCode();
            hash ^= repeated.GetHashCode();
            hash ^= repeatSpan.GetHashCode();
            hash ^= repeatYears.GetHashCode();
            hash ^= repeatMonths.GetHashCode();
            hash ^= next.GetHashCode();
            return hash;
        }
    }

    public class PlanumTask
    {
        public Guid Id { get; set; }
        public bool Complete { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public HashSet<Deadline> Deadlines { get; set; } = new HashSet<Deadline>();
        public HashSet<Guid> Children { get; set; } = new HashSet<Guid>();
        public HashSet<Guid> Parents { get; set; } = new HashSet<Guid>();

        public PlanumTask(Guid? id = null,
                bool complete = false,
                string name = "",
                string description = "",
                IEnumerable<Deadline>? deadlines = null,
                IEnumerable<Guid>? children = null,
                IEnumerable<Guid>? parents = null)
        {
            Id = id is null ? new Guid() : (Guid)id;
            Complete = complete;
            Name = name;
            Description = description;
            if (deadlines is not null)
                Deadlines = deadlines.ToHashSet();
            if (children is not null)
                Children = children.ToHashSet();
            if (parents is not null)
                Parents = parents.ToHashSet();
        }

        public void AddChildren(IEnumerable<Guid> children) => Children = Children.Concat(children).ToHashSet();
        public void AddChild(Guid child) => Children.Add(child);
        public void AddParents(IEnumerable<Guid> parents) => Parents = Parents.Concat(parents).ToHashSet();
        public void AddParent(Guid parent) => Parents.Add(parent);

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            return Equals((PlanumTask)obj);
        }

        public bool Equals(PlanumTask compared)
        {
            if (Id != compared.Id || Complete != compared.Complete || Name != compared.Name || Description != compared.Description)
                return false;

            if (Deadlines.Except(compared.Deadlines).Any()) return false;
            if (Children.Except(compared.Children).Any()) return false;
            if (Parents.Except(compared.Parents).Any()) return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= Complete.GetHashCode();
            hash ^= Name.GetHashCode();
            hash ^= Description.GetHashCode();
            foreach (var deadline in Deadlines)
                hash ^= deadline.GetHashCode();
            foreach (var parent in Parents)
                hash ^= parent.GetHashCode();
            foreach (var child in Children)
                hash ^= child.GetHashCode();
            return hash;
        }

        public bool Warning() => Deadlines.Where(x => x.Warning() && !x.InProgress() && !x.Overdue()).Any();
        public bool InProgress() => Deadlines.Where(x => !x.Warning() && x.InProgress() && !x.Overdue()).Any();
        public bool Overdue() => Deadlines.Where(x => !x.Warning() && !x.InProgress() && x.Overdue()).Any();

        public static IEnumerable<PlanumTask> FillRelatives(IEnumerable<PlanumTask> tasks)
        {
            Dictionary<Guid, IEnumerable<Guid>> parentToChildren = new Dictionary<Guid, IEnumerable<Guid>>();
            Dictionary<Guid, IEnumerable<Guid>> childToParents = new Dictionary<Guid, IEnumerable<Guid>>();

            foreach (var task in tasks)
            {
                if (!parentToChildren.ContainsKey(task.Id))
                    parentToChildren[task.Id] = new List<Guid>();
                if (!childToParents.ContainsKey(task.Id))
                    childToParents[task.Id] = new List<Guid>();
                parentToChildren[task.Id] = parentToChildren[task.Id].Concat(task.Children);
                foreach (var parentId in task.Parents)
                    parentToChildren[parentId] = parentToChildren[parentId].Append(task.Id);
                childToParents[task.Id] = childToParents[task.Id].Concat(task.Parents);
                foreach (var childId in task.Children)
                    childToParents[childId] = childToParents[childId].Append(task.Id);
            }

            foreach (var task in tasks)
            {
                task.Children = parentToChildren[task.Id].ToHashSet();
                task.Parents = childToParents[task.Id].ToHashSet();
            }

            return tasks;
        }
    }
}
