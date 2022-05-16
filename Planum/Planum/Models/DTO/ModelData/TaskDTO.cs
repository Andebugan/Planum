using System;
using System.Collections.Generic;

namespace Planum.Models.DTO.ModelData
{
    public class TaskDTO
    {
        public int Id { get; }
        public int UserId { get; }
        public int ParentId { get; }
        public string Name { get; }
        public string Description { get; }

        private List<int> tagIds = new List<int>();
        public IReadOnlyList<int> TagIds => tagIds;
        public DateTime StartTime { get; }
        public DateTime Deadline { get; }
        public bool IsRepeated { get; }
        public TimeSpan RepeatPeriod { get; }

        public TaskDTO(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, int userId = -1,
            string name = "", string description = "", int parentId = -1, bool isRepeated = false)
        {
            if (parentId == -1)
                parentId = id;

            Id = id;
            UserId = userId;
            ParentId = parentId;
            Name = name;
            Description = description;

            tagIds = (List<int>)TagIds;
            StartTime = startTime;
            Deadline = deadline;
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod;
        }
    }
}
