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

        public TaskDTO(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, int userId = -1,
            string description = "", bool isRepeated = false)
        {

            Id = id;
            UserId = userId;
            parentIds = (List<int>)ParentIds;
            childIds = (List<int>)ChildIds;
            tagIds = (List<int>)TagIds;
            Name = name;
            Description = description;
            Timed = timed;

            tagIds = (List<int>)TagIds;
            StartTime = startTime;
            Deadline = deadline;
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod;
        }
    }
}
