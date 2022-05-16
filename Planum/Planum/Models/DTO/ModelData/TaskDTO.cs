using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.BuisnessLayer.Entities;

namespace Planum.Models.DTO.ModelData
{
    public class TaskDTO
    {
        public int Id { get; protected set; }
        public int UserId { get; protected set; }
        public int ParentId { get; protected set; }
        public string? Name { get; protected set; }
        public string? Description { get; protected set; }

        private List<int> tagIds = new List<int>();
        public List<int> TagIds { get { return tagIds; } protected set { tagIds = value; } }

        public bool Timed { get; protected set; }
        public DateTime StartTime { get; protected set; }
        public DateTime Deadline { get; protected set; }
        public bool IsRepeated { get; protected set; }
        public TimeSpan RepeatPeriod { get; protected set; }

        public TaskDTO(int id, int userId, string? name, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, List<int> tagIds, bool timed = false, string? description = null,
            int parentId = -1, bool isRepeated = false)
        {
            if (parentId == -1)
                parentId = id;

            Id = id;
            UserId = userId;
            ParentId = parentId;
            Name = name;
            Description = description;
            TagIds = tagIds;
            Timed = timed;
            StartTime = startTime;
            Deadline = deadline;
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod;
        }

        public TaskDTO(int id, int userId, string? name, List<int> tagIds, bool timed = false,
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
            Timed = timed;
            StartTime = startTime;
            Deadline = deadline;
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod;
        }

        public TaskDTO(int id, int userId, string? name, bool timed = false,
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
            Timed = timed;
            StartTime = startTime;
            Deadline = deadline;
            IsRepeated = isRepeated;
            RepeatPeriod = repeatPeriod;
        }

        public override bool Equals(Object? obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                TaskDTO task = (TaskDTO)obj;
                if (task.Id != Id)
                    return false;
                if (task.UserId != UserId)
                    return false;
                if (task.ParentId != ParentId)
                    return false;
                if (task.Name != Name)
                    return false;
                if (task.Description != Description)
                    return false;
                if (task.Timed != Timed)
                    return false;
                if (Math.Abs((StartTime - task.StartTime).TotalSeconds) > 1)
                    return false;
                if (Math.Abs((Deadline - task.Deadline).TotalSeconds) > 1)
                    return false;
                if (!task.TagIds.SequenceEqual(TagIds))
                    return false;
                if (task.IsRepeated != IsRepeated)
                    return false;
                if (Math.Abs((RepeatPeriod - task.RepeatPeriod).TotalSeconds) > 1)
                    return false;
                return true;
            }
        }

        public override string ToString()
        {
            return String.Format("TaskDTO({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})",
                Id, UserId, ParentId, Name, Description,
                Timed, StartTime.ToString(), Deadline.ToString(), TagIds.ToString(), IsRepeated,
                RepeatPeriod.ToString());
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
