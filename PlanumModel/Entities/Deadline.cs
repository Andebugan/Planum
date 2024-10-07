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
        public RepeatSpan repeatSpan;

        public HashSet<Guid> next;

        public Deadline(bool enabled = false,
                DateTime? deadline = null,
                TimeSpan? warningTime = null,
                TimeSpan? duration = null,
                bool repeated = false,
                RepeatSpan? repeatSpan = null,
                HashSet<Guid>? next = null)
        {
            Id = Guid.NewGuid();
            this.enabled = enabled;
            this.deadline = deadline is null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0) : (DateTime)deadline;
            this.warningTime = warningTime is null ? TimeSpan.Zero : (TimeSpan)warningTime;

            this.duration = duration is null ? TimeSpan.Zero : (TimeSpan)duration;

            this.repeated = repeated;
            this.repeatSpan = (RepeatSpan)(repeatSpan is null ? new RepeatSpan() : repeatSpan);
            this.next = next is null ? new HashSet<Guid>() : next;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            return Equals((Deadline)obj);
        }

        public PlanumTaskStatus GetDeadlineStatus()
        {
            if (!enabled)
                return PlanumTaskStatus.COMPLETE;
            else if (DateTime.Now > deadline)
                return PlanumTaskStatus.OVERDUE;
            else if ((deadline - duration) < DateTime.Now)
                return PlanumTaskStatus.IN_PROGRESS;
            else if ((deadline - duration - warningTime) < DateTime.Now)
                return PlanumTaskStatus.WARNING;
            return PlanumTaskStatus.NOT_STARTED;
        }

        public bool Equals(Deadline compared) => GetHashCode() == this.GetHashCode();

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
            hash ^= next.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            string result = "Deadline(";
            result += $"id={Id.ToString()}, ";
            result += $"enabled={enabled.ToString()}, ";
            result += $"deadline={deadline.ToString()}";
            if (warningTime != TimeSpan.Zero)
                result += $", warningTime={warningTime.ToString()}";
            if (duration != TimeSpan.Zero)
                result += $", duration={duration.ToString()}";
            if (repeated)
                result += $", repeated={repeated.ToString()}";
            result += $", repeatSpan={repeatSpan.ToString()}";
            if (next.Any())
            {
                result += ", next={";
                foreach (var id in next)
                    result += id.ToString() + ", ";
                if (next.Count() > 0)
                    result = result.Remove(result.Length - 2, 2);
                result += "}";
            }
            result += ")";
            return result;
        }
    }
}
