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
        public bool Archived { get; set; } = false;

        private List<int> statusQueueIds = new List<int>();
        public IReadOnlyList<int> StatusQueueIds => statusQueueIds;

        private int currentStatusIndex = 0;
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
                if (StatusQueueIds.Count == 0)
                    currentStatusIndex = 0;
            }
        }

        public Task(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, int userId = -1,
            string description = "", bool isRepeated = false, bool archived = false, IReadOnlyList<int>? StatusQueueIds = null)
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


        public void NextStatus()
        {
            if (CurrentStatusIndex < StatusQueueIds.Count - 1)
                currentStatusIndex++;
        }

        public void PreviousStatus()
        {
            if (CurrentStatusIndex > 0)
                currentStatusIndex--;
        }

        public void SetStatusIndex(int index)
        {
            CurrentStatusIndex = index;
        }

        public void AddStatus(int statusId, int position = -1)
        {
            if (position != -1)
                statusQueueIds.Add(statusId);
            else
                statusQueueIds.Insert(position, statusId);
        }

        public void RemoveStatus(int statusId)
        {
            statusQueueIds.RemoveAll(x => x == statusId);
        }

        public void RemoveStatusAtPosition(int position)
        {
            statusQueueIds.RemoveAt(position);
        }
    }
}
