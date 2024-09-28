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
            result += $"deadline={deadline.ToString()}";
            if (warningTime != TimeSpan.Zero)
                result += $", warningTime={warningTime.ToString()}";
            if (duration != TimeSpan.Zero)
                result += $", duration={duration.ToString()}";
            if (repeated)
                result += $", repeated={repeated.ToString()}";
            if (repeatSpan != TimeSpan.Zero)
                result += $", repeatSpan={repeatSpan.ToString()}";
            if (repeatYears > 0)
                result += $", repeatYears={repeatYears.ToString()}";
            if (repeatMonths > 0)
                result += $", repeatMonths={repeatMonths.ToString()}";
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
