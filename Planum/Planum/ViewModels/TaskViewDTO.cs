using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ViewModels
{
    public class TaskViewDTO
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
        public string StartTime { get; }
        public string Deadline { get; }
        public bool IsRepeated { get; }
        public string RepeatPeriod { get; }
        public bool Archived { get; set; } = false;

        public TaskViewDTO(int id, DateTime startTime, DateTime deadline,
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
            StartTime = startTime.ToString();
            Deadline = deadline.ToString();
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod.ToString();
        }
    }
}
