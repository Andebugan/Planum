﻿#nullable enable

namespace Planum.Model.Entities
{
    public enum TaskStatus
    {
        DISABLED,
        NOT_STARTED,
        IN_PROGRESS,
        WARNING,
        OVERDUE
    }

    public static class DefaultTags
    {
        public static string Complete = "complete";
        public static string Checklist = "checklist";
    }

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

        public TaskStatus GetDeadlineStatus()
        {
            if (!enabled)
                return TaskStatus.DISABLED;
            else if ((deadline - duration - warningTime) < DateTime.Now)
                return TaskStatus.WARNING;
            else if ((deadline - duration) < DateTime.Now)
                return TaskStatus.IN_PROGRESS;
            else if (DateTime.Now > deadline)
                return TaskStatus.OVERDUE;
            return TaskStatus.NOT_STARTED;
        }

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

        public override string ToString()
        {
            string result = "Deadline(";
            result += $"id={Id.ToString()}, "; 
            result += $"enabled={enabled.ToString()}, ";
            result += $"deadline={deadline.ToString()}, ";
            result += $"warningTime={warningTime.ToString()}, ";
            result += $"duration={duration.ToString()}, ";
            result += $"repeated={repeated.ToString()}, ";
            result += $"repeatSpan={repeatSpan.ToString()}, ";
            result += $"repeatYears={repeatYears.ToString()}, ";
            result += $"repeatMonths={repeatMonths.ToString()}, ";
            result += "next={";
            foreach (var id in next)
                result += id.ToString() + ", "; 
            if (next.Count() > 0)
                result = result.Remove(result.Length - 2, 2);
            result += "})";
            return result;
        }
    }

    public class PlanumTask
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string SaveFile { get; set; } = "./tasks.md";

        public HashSet<string> Tags { get; set; } = new HashSet<string>();
        public HashSet<Deadline> Deadlines { get; set; } = new HashSet<Deadline>();
        public HashSet<Guid> Children { get; set; } = new HashSet<Guid>();
        public HashSet<Guid> Parents { get; set; } = new HashSet<Guid>();

        public PlanumTask(Guid? id = null,
                string name = "",
                string description = "",
                IEnumerable<Deadline>? deadlines = null,
                IEnumerable<Guid>? children = null,
                IEnumerable<Guid>? parents = null,
                IEnumerable<string>? tags = null,
                string saveFile = "./tasks.md")
        {
            Id = id is null ? new Guid() : (Guid)id;
            Name = name;
            Description = description;
            if (deadlines is not null)
                Deadlines = deadlines.ToHashSet();
            if (children is not null)
                Children = children.ToHashSet();
            if (parents is not null)
                Parents = parents.ToHashSet();
            if (tags is not null)
                Tags = tags.ToHashSet();
            SaveFile = saveFile;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            return Equals((PlanumTask)obj);
        }

        public bool Equals(PlanumTask compared)
        {
            if (Id != compared.Id || Name != compared.Name || Description != compared.Description)
                return false;

            if (Deadlines.Except(compared.Deadlines).Any()) return false;
            if (Children.Except(compared.Children).Any()) return false;
            if (Parents.Except(compared.Parents).Any()) return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= Name.GetHashCode();
            hash ^= Description.GetHashCode();

            foreach (var deadline in Deadlines)
                hash ^= deadline.GetHashCode();
            foreach (var parent in Parents)
                hash ^= parent.GetHashCode();
            foreach (var child in Children)
                hash ^= child.GetHashCode();
            foreach (var tag in Tags)
                hash ^= tag.GetHashCode();
            hash ^= SaveFile.GetHashCode();
            return hash;
        }

        public TaskStatus GetTaskStatus()
        {
            var statuses = Deadlines.Select(x => x.GetDeadlineStatus());
            if (statuses.Any(x => x == TaskStatus.OVERDUE))
                return TaskStatus.OVERDUE;
            else if (statuses.Any(x => x == TaskStatus.IN_PROGRESS))
                return TaskStatus.IN_PROGRESS;
            else if (statuses.Any(x => x == TaskStatus.WARNING))
                return TaskStatus.WARNING;
            else if (statuses.Any(x => x == TaskStatus.NOT_STARTED))
                return TaskStatus.NOT_STARTED;
            else 
                return TaskStatus.DISABLED;
        }

        public static Dictionary<Guid, TaskStatus> GetTaskStatuses(IEnumerable<PlanumTask> tasks)
        {
            Dictionary<Guid, TaskStatus> statuses = new Dictionary<Guid, TaskStatus>();
            foreach (var task in tasks)
                statuses[task.Id] = task.GetTaskStatus();

            foreach (var task in tasks)
                foreach (var child in task.Children)
                    if (statuses[child] > statuses[task.Id])
                    {
                        statuses[task.Id] = statuses[child];
                        break;
                    }

            return statuses;
        }

        public static IEnumerable<PlanumTask> UpdateRelatives(IEnumerable<PlanumTask> tasks)
        {
            Dictionary<Guid, HashSet<Guid>> parentToChildren = new Dictionary<Guid, HashSet<Guid>>();
            Dictionary<Guid, HashSet<Guid>> childToParents = new Dictionary<Guid, HashSet<Guid>>();

            foreach (var task in tasks)
            {
                if (!parentToChildren.ContainsKey(task.Id))
                    parentToChildren[task.Id] = new HashSet<Guid>();
                if (!childToParents.ContainsKey(task.Id))
                    childToParents[task.Id] = new HashSet<Guid>();

                foreach (var child in task.Children)
                {
                    parentToChildren[task.Id].Add(child);
                    if (!childToParents.ContainsKey(child))
                        childToParents[child] = new HashSet<Guid>();
                    childToParents[child].Add(task.Id);
                }

                foreach (var parent in task.Parents)
                {
                    childToParents[task.Id].Add(parent);
                    if (!parentToChildren.ContainsKey(parent))
                        parentToChildren[parent] = new HashSet<Guid>();
                    parentToChildren[parent].Add(task.Id);
                }
            }

            List<PlanumTask> newTasks = new List<PlanumTask>();

            foreach (var task in tasks)
            {
                task.Children = parentToChildren[task.Id];
                task.Parents = childToParents[task.Id];
                newTasks.Add(task);
            }
            return newTasks;
        }

        public override string ToString()
        {
            string result = "PlanumTask(";
            result += "Id=" + Id.ToString() + ", ";
            result += "Name=" + Name.ToString() + ", ";
            result += "Description=" + Description.ToString() + ", ";

            result += "Tags={";
            foreach (var item in Tags)
                result += item.ToString() + ", ";
            if (Tags.Count() > 0)
                result = result.Remove(result.Length - 2, 2);
            result += "}, ";

            result += "Deadlines={";
            foreach (var item in Deadlines)
                result += item.ToString() + ", ";
            if (Deadlines.Count() > 0)
                result = result.Remove(result.Length - 2, 2);
            result += "}, ";

            result += "Children={";
            foreach (var item in Children)
                result += item.ToString() + ", ";
            if (Children.Count() > 0)
                result = result.Remove(result.Length - 2, 2);
            result += "}, ";

            result += "Parents={";
            foreach (var item in Parents)
                result += item.ToString() + ", ";
            if (Parents.Count() > 0)
                result = result.Remove(result.Length - 2, 2);
            result += "}, ";

            result += "SaveFile=" + SaveFile;

            return result;
        }
    }
}
