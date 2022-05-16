using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Entities
{
    public class Task
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

        public Task(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, List<int> TagIds, int userId = -1,
            string name = "", string description = "", int parentId = -1, bool isRepeated = false)
        {
            if (parentId == -1)
                parentId = id;

            Id = id;
            UserId = userId;
            ParentId = parentId;
            Name = name;
            Description = description;

            tagIds = TagIds;
            StartTime = startTime;
            Deadline = deadline;
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod;
        }

        public void AddTag(int tagId)
        {
            if (tagIds.Any(x => x == tagId))
                return;
            tagIds.Add(tagId);
        }

        public void RemoveTag(int id)
        {
            foreach (var node in TagIds)
            {
                if (node == id)
                {
                    tagIds.Remove(node);
                    return;
                }
            }
        }

        public void ClearTags()
        {
            tagIds.Clear();
        }
    }
}
