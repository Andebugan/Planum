#nullable enable

namespace Planum.Model.Entities
{
    public enum PlanumTaskStatus
    {
        DISABLED,
        COMPLETE,
        NOT_STARTED,
        WARNING,
        IN_PROGRESS,
        OVERDUE
    }

    public static class DefaultTags
    {
        public static string Complete = "complete";
        public static string Checklist = "checklist";
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
            return GetHashCode() == this.GetHashCode();
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

        public PlanumTaskStatus GetTaskStatus()
        {
            var statuses = Deadlines.Select(x => x.GetDeadlineStatus());
            if (Tags.Contains(DefaultTags.Complete))
                return PlanumTaskStatus.COMPLETE;
            if (!statuses.Any())
                return PlanumTaskStatus.DISABLED;
            else if (statuses.Any(x => x == PlanumTaskStatus.OVERDUE))
                return PlanumTaskStatus.OVERDUE;
            else if (statuses.Any(x => x == PlanumTaskStatus.IN_PROGRESS))
                return PlanumTaskStatus.IN_PROGRESS;
            else if (statuses.Any(x => x == PlanumTaskStatus.WARNING))
                return PlanumTaskStatus.WARNING;
            else if (statuses.Any(x => x == PlanumTaskStatus.NOT_STARTED))
                return PlanumTaskStatus.NOT_STARTED;
            else if (statuses.All(x => x == PlanumTaskStatus.COMPLETE))
                return PlanumTaskStatus.COMPLETE;
            return PlanumTaskStatus.DISABLED;
        }

        public static Dictionary<Guid, PlanumTaskStatus> GetTaskStatuses(IEnumerable<PlanumTask> tasks)
        {
            Dictionary<Guid, PlanumTaskStatus> statuses = new Dictionary<Guid, PlanumTaskStatus>();
            if (!tasks.Any())
                return statuses;

            foreach (var task in tasks)
                statuses[task.Id] = task.GetTaskStatus();

            var topLevelTasks = tasks.Where(x => x.Parents.Count() == 0 && x.Children.Count() > 0).Select(x => x.Id).ToHashSet();
            var updatedTasks = new HashSet<Guid>();

            foreach (var taskId in topLevelTasks)
            {
                updatedTasks.Add(taskId);
                GetTaskStatusesRecursive(statuses, updatedTasks, taskId, tasks);
            }

            return statuses;
        }

        protected static void GetTaskStatusesRecursive(Dictionary<Guid, PlanumTaskStatus> statuses, HashSet<Guid> updatedTasks, Guid currentTaskId, IEnumerable<PlanumTask> tasks)
        {
            var task = tasks.First(x => x.Id == currentTaskId);
            var childrenIds = task.Children.Where(x => !updatedTasks.Contains(x));
            if (!childrenIds.Any())
                return;
            foreach (var childId in childrenIds)
            {
                updatedTasks.Add(childId);
                GetTaskStatusesRecursive(statuses, updatedTasks, childId, tasks);
                if (statuses[childId] > statuses[currentTaskId])
                    statuses[currentTaskId] = statuses[childId];
            }

            if (task.Tags.Contains(DefaultTags.Complete))
                statuses[currentTaskId] = PlanumTaskStatus.COMPLETE;
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
            result += "Id=" + Id.ToString();
            if (Name.ToString() != "")
                result += ", Name=" + Name.ToString();
            if (Description.ToString() != "")
                result += ", Description=" + Description.ToString();

            if (Tags.Any())
            {
                result += ", Tags={";
                foreach (var item in Tags)
                    result += item.ToString() + ", ";
                if (Tags.Count() > 0)
                    result = result.Remove(result.Length - 2, 2);
                result += "}";
            }

            if (Deadlines.Any())
            {
                result += ", Deadlines={";
                foreach (var item in Deadlines)
                    result += item.ToString() + ", ";
                if (Deadlines.Count() > 0)
                    result = result.Remove(result.Length - 2, 2);
                result += "}";
            }

            if (Children.Any())
            {
                result += ", Children={";
                foreach (var item in Children)
                    result += item.ToString() + ", ";
                if (Children.Count() > 0)
                    result = result.Remove(result.Length - 2, 2);
                result += "}";
            }

            if (Parents.Any())
            {
                result += ", Parents={";
                foreach (var item in Parents)
                    result += item.ToString() + ", ";
                if (Parents.Count() > 0)
                    result = result.Remove(result.Length - 2, 2);
                result += "}";
            }

            if (SaveFile != "")
                result += ", SaveFile=" + SaveFile;
            result += ")";

            return result;
        }
    }
}
