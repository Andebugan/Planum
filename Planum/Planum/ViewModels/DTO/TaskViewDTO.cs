using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ViewModels
{
    public class TaskViewDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ParentIds { get; set; }
        public string ChildIds { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string TagIds { get; set; }
        public bool Timed { get; set; }
        public string StartTime { get; set; }
        public string Deadline { get; set; }
        public bool IsRepeated { get; set; }
        public string RepeatPeriod { get; set; }
        public string StatusQueueIds { get; set; }
        public bool HasStatus { get; set; }
        public int CurrentStatusIndex { get; set; }
        public int CurrentStatus { get; set; }
        public bool Archived { get; set; }

        public TaskViewDTO(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, int userId = -1,
            string description = "", bool isRepeated = false, bool archived = false,
            IReadOnlyList<int>? StatusQueueIds = null, int currentStatusIndex = 0)
        {

            Id = id;
            UserId = userId;
            this.ParentIds = string.Join(" ", ParentIds);
            this.ChildIds = string.Join(" ", ChildIds);
            this.TagIds = string.Join(" ", TagIds);
            Name = name;
            Description = description;
            Timed = timed;
            StartTime = startTime.ToString();
            Deadline = deadline.ToString();
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod.ToString();
            CurrentStatusIndex = currentStatusIndex;
            if (StatusQueueIds == null || StatusQueueIds.Count == 0)
            {
                this.StatusQueueIds = "";
                HasStatus = false;
            }
            else
            {
                HasStatus = true;
                CurrentStatus = StatusQueueIds[currentStatusIndex];
                this.StatusQueueIds = string.Join(" ", StatusQueueIds);
            }
            Archived = archived;
        }
    }
}
