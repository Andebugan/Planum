using System;
using System.Collections.Generic;
using System.Linq;

namespace Planum.Models.DTO
{
    public class TaskDTO
    {
        public int Id { get; }
        public int UserId { get; }

        private List<int> parentIds = new List<int>();
        public IReadOnlyList<int> ParentIds => parentIds;

        private List<int> childIds = new List<int>();
        public IReadOnlyList<int> ChildIds => childIds;
        public string Name { get; }
        public string Description { get; }

        private List<int> tagIds = new List<int>();
        public IReadOnlyList<int> TagIds => tagIds;
        public bool Timed { get; }
        public DateTime StartTime { get; }
        public DateTime Deadline { get; }
        public bool IsRepeated { get; }
        public TimeSpan RepeatPeriod { get; }
        public bool Archived { get; set; } = false;

        private List<int> statusQueueIds = new List<int>();
        public IReadOnlyList<int> StatusQueueIds => statusQueueIds;

        private int currentStatusIndex;
        public int CurrentStatusIndex
        {
            get
            {
                return currentStatusIndex;
            }

            set
            {
                if (value >= 0 && value < StatusQueueIds.Count)
                    currentStatusIndex = value;
            }
        }

        public TaskDTO(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, int userId = -1,
            string description = "", bool isRepeated = false, bool archived = false,
            IReadOnlyList<int>? StatusQueueIds = null, int currentStatusIndex = 0)
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Task name can not be null or empty", nameof(name));

            Id = id;
            UserId = userId;
            Name = name;
            Description = description;
            Timed = timed;
            parentIds = (List<int>)ParentIds;
            childIds = (List<int>)ChildIds;
            tagIds = (List<int>)TagIds;
            StartTime = startTime;
            Deadline = deadline;
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod;
            Archived = archived;
            if (StatusQueueIds != null)
                statusQueueIds = (List<int>)StatusQueueIds;
            CurrentStatusIndex = currentStatusIndex;
        }
    }
}
