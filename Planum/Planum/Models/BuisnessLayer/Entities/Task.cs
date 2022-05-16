using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Entities
{
    public class Task
    {
        public int Id { get; protected set; }
        public int UserId { get; protected set; }
        public int ParentId { get; protected set; }
        public string? Name { get; protected set; }
        public string? Description { get; protected set; }

        private List<int> tagIds = new List<int>();
        public List<int> TagIds { get { return tagIds; } protected set { tagIds = value; } }
        public DateTime StartTime { get; protected set; }
        public DateTime Deadline { get; protected set; }
        public bool IsRepeated { get; protected set; }
        public TimeSpan RepeatPeriod { get; protected set; }

        public Task(int id, int userId, string? name, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, List<int> tagIds, string? description = null, int parentId = -1, bool isRepeated = false)
        {
            if (parentId == -1)
                parentId = id;

            Id = id;
            UserId = userId;
            ParentId = parentId;
            Name = name;
            Description = description;
            TagIds = tagIds;
            StartTime = startTime;
            Deadline = deadline;
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod;
        }

        public Task(int id, int userId, string? name, List<int> tagIds, 
            string? description = null, int parentId = -1, bool isRepeated = false)
        {
            DateTime startTime = DateTime.MinValue;
            DateTime deadline = DateTime.MinValue;
            TimeSpan repeatPeriod = TimeSpan.Zero;

            if (parentId == -1)
                parentId = id;

            Id = id;
            UserId = userId;
            ParentId = parentId;
            Name = name;
            Description = description;
            TagIds = tagIds;
            StartTime = startTime;
            Deadline = deadline;
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod;
        }

        public Task(int id, int userId, string? name,
            string? description = null, int parentId = -1, bool isRepeated = false)
        {
            List<int> tagIds = new List<int>();
            DateTime startTime = DateTime.MinValue;
            DateTime deadline = DateTime.MinValue;
            TimeSpan repeatPeriod = TimeSpan.Zero;

            if (parentId == -1)
                parentId = id;

            Id = id;
            UserId = userId;
            ParentId = parentId;
            Name = name;
            Description = description;
            TagIds = tagIds;
            StartTime = startTime;
            Deadline = deadline;
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod;
        }

        public Task(Task task)
        {
            Id = task.Id;
            UserId = task.UserId;
            ParentId = task.ParentId;
            Name = task.Name;
            Description = task.Description;
            TagIds = tagIds;
            StartTime = task.StartTime;
            Deadline = task.Deadline;
            IsRepeated = task.IsRepeated;
            RepeatPeriod = task.RepeatPeriod;
        }

        public void AddTag(int tagId)
        {
            if (TagIds.Any(x => x == tagId))
                return;
            TagIds.Add(tagId);
        }

        public void RemoveTag(int id)
        {
            foreach (var node in TagIds)
            {
                if (node == id)
                {
                    TagIds.Remove(node);
                    return;
                }
            }
        }

        public void ClearTags()
        {
            TagIds.Clear();
        }

        public void AddParent(int parentId)
        {
            ParentId = parentId;
        }

        public void RemoveParent()
        {
            ParentId = Id;
        }
    }
}
