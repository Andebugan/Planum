using System;
using System.Collections.Generic;
using System.Linq;

namespace Planum.Models.BuisnessLogic.Entities
{
    public class Task
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

        public Task(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, int userId = -1,
            string description = "", bool isRepeated = false)
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

        public void AddParent(int parentId)
        {
            if (parentIds.Any(x => x == parentId))
                return;
            parentIds.Add(parentId);
        }

        public void RemoveParent(int id)
        {
            foreach (var node in ParentIds)
            {
                if (node == id)
                {
                    parentIds.Remove(node);
                    return;
                }
            }
        }

        public void ClearParents()
        {
            parentIds.Clear();
        }

        public void AddChild(int childId)
        {
            if (childIds.Any(x => x == childId))
                return;
            childIds.Add(childId);
        }

        public void RemoveChild(int id)
        {
            foreach (var node in ChildIds)
            {
                if (node == id)
                {
                    childIds.Remove(node);
                    return;
                }
            }
        }

        public void ClearChildIds()
        {
            childIds.Clear();
        }
    }
}
