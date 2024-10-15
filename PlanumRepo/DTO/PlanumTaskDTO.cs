using Planum.Model.Entities;
using Planum.Parser;

namespace Planum.Repository
{
    public class PlanumTaskDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public string SaveFile { get; set; } = "./tasks.md";

        public HashSet<string> Tags { get; set; } = new HashSet<string>();
        public HashSet<DeadlineDTO> Deadlines { get; set; } = new HashSet<DeadlineDTO>();

        public HashSet<Tuple<string, string>> Children { get; set; } = new HashSet<Tuple<string, string>>();
        public HashSet<Tuple<string, string>> Parents { get; set; } = new HashSet<Tuple<string, string>>();

        public PlanumTask ToPlanumTask(Dictionary<Guid, string> tasks)
        {
            PlanumTask task = new PlanumTask();
            task.Id = Id;
            task.Name = Name;
            task.Description = Description;

            task.SaveFile = SaveFile;

            task.Tags = Tags.ToHashSet();
            task.Deadlines = Deadlines.Select(x => x.ToDeadline(task.Id, task.Name, tasks)).ToHashSet();

            foreach (var childItem in Children)
            {
                var matches = TaskValueParser.ParseIdentity(childItem.Item1, childItem.Item2, tasks);
                if (matches.Count() == 0)
                    throw new TaskRepoException($"Unable to find child task ({childItem.Item1}|{childItem.Item2}) for task ({Id.ToString()}|{Name})");
                if (matches.Count() != 1)
                    throw new TaskRepoException($"Unable uniquely to find child task ({childItem.Item1}|{childItem.Item2}) for task ({Id.ToString()}|{Name})");
                task.Children.Add(matches.First());
            }

            foreach (var parentItem in Parents)
            {
                var matches = TaskValueParser.ParseIdentity(parentItem.Item1, parentItem.Item2, tasks);
                if (matches.Count() == 0)
                    throw new TaskRepoException($"Unable to find parent task ({parentItem.Item1}|{parentItem.Item2}) for task ({Id.ToString()}|{Name})");
                if (matches.Count() != 1)
                    throw new TaskRepoException($"Unable uniquely to find child task ({parentItem.Item1}|{parentItem.Item2}) for task ({Id.ToString()}|{Name})");
                task.Parents.Add(matches.First());
            }

            return task;
        }

        public override int GetHashCode()
        {
            var hashCode = HashCode.Combine(Id.GetHashCode(), Name.GetHashCode(), Description.GetHashCode(), SaveFile.GetHashCode());
            foreach (var tag in Tags)
                hashCode ^= tag.GetHashCode();
            foreach (var deadline in Deadlines)
                hashCode ^= deadline.GetHashCode();
            foreach (var parent in Parents)
                hashCode ^= parent.GetHashCode();
            foreach (var child in Children)
                hashCode ^= child.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
