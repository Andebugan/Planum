using Planum.Model.Entities;
using Planum.Parser;

namespace Planum.Repository
{
    public class DeadlineDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool enabled = false;
        public DateTime deadline = DateTime.MinValue;
        public TimeSpan warningTime = TimeSpan.Zero;
        public TimeSpan duration = TimeSpan.Zero;

        public bool repeated = false;
        public RepeatSpan repeatSpan = new RepeatSpan();

        public HashSet<Tuple<string, string>> next = new HashSet<Tuple<string, string>>();

        public Deadline ToDeadline(Guid taskId, string taskName, Dictionary<Guid, string> tasks)
        {
            Deadline deadline = new Deadline();
            deadline.enabled = enabled;
            deadline.deadline = this.deadline;
            deadline.warningTime = warningTime;
            deadline.duration = duration;

            deadline.repeated = repeated;
            deadline.repeatSpan = repeatSpan;

            foreach (var nextItem in next)
            {
                var matches = TaskValueParser.ParseIdentity(nextItem.Item1, nextItem.Item2, tasks);
                if (matches.Count() != 1)
                    throw new TaskRepoException($"Unable uniquely to find next task ({nextItem.Item1}|{nextItem.Item2}) for deadline ({Id}) for task ({taskId.ToString()}|{taskName})");
                deadline.next.Add(matches.First());
            }

            return deadline;
        }
    }
}
