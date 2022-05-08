using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.BuisnessLayer.Entities;

namespace Planum.Models.DTO.ModelData
{
    struct TaskParamsDTO
    {
        public int id;
        public int userId;
        public int parentId;
        public string name;
        public string description;
        public List<int> tags;
        public DateTime startTime;
        public DateTime deadline;
        public bool isRepeated;
        public TimeSpan repeatPeriod;
    };

    internal class TaskDTO
    {
        private int _id;
        public int Id { get { return _id; } set { _id = value; } }

        private int _userId;
        public int UserId { get { return _userId; } set { _userId = value; } }

        private int _parentId;
        public int ParentId { get { return _parentId; } set { _parentId = value; } }

        private string _name = "task";
        public string Name { get { return _name; } set { _name = value; } }

        private string _description = "";
        public string Description { get { return _description; } set { _description = value; } }

        private List<int> tags = new List<int>();
        public List<int> Tags { get { return tags; } set { tags = value; } }

        private DateTime _startTime;
        public DateTime StartTime { get { return _startTime; } set { _startTime = value; } }

        private DateTime _deadline;
        public DateTime Deadline { get { return _deadline; } set { _deadline = value; } }

        private bool _isRepeated;
        public bool IsRepeated { get { return _isRepeated; } set { _isRepeated = value; } }

        private TimeSpan _repeatPeriod;
        public TimeSpan RepeatPeriod { get { return _repeatPeriod; } set { _repeatPeriod = value; } }

        public TaskDTO(TaskParamsDTO taskParams)
        {
            Id = taskParams.id;
            UserId = taskParams.userId;
            ParentId = taskParams.parentId;
            Name = taskParams.name;
            Description = taskParams.description;
            Tags = taskParams.tags;
            StartTime = taskParams.startTime;
            Deadline = taskParams.deadline;
            IsRepeated = taskParams.isRepeated;
            RepeatPeriod = taskParams.repeatPeriod;
        }
    }
}
