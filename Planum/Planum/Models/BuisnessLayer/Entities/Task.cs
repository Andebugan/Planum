using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Entities
{
    struct TaskParams
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
    }

    internal class Task
    {
        protected int _id;
        public int Id { get { return _id; } }

        protected int _userId;
        public int UserId { get { return _userId; } }

        protected int _parentId;
        public int ParentId { get { return _parentId; } }

        protected string _name = "task";
        public string Name { get { return _name; } }

        protected string _description = "";
        public string Description { get { return _description; } }

        protected List<int> _tags = new List<int>();
        public List<int> Tags { get { return _tags; } }

        protected DateTime _startTime;
        public DateTime StartTime { get { return _startTime; } }

        protected DateTime _deadline;
        public DateTime Deadline { get { return _deadline; } }

        protected bool _isRepeated;
        public bool IsRepeated { get { return _isRepeated; } }

        protected TimeSpan _repeatPeriod;
        public TimeSpan RepeatPeriod { get { return _repeatPeriod; } }

        public Task(TaskParams taskParams)
        {
            _id = taskParams.id;
            _userId = taskParams.userId;
            _parentId = taskParams.parentId;
            _name = taskParams.name;
            _description = taskParams.description;
            _tags = taskParams.tags;
            _startTime = taskParams.startTime;
            _deadline = taskParams.deadline;
            _isRepeated = taskParams.isRepeated;
            _repeatPeriod = taskParams.repeatPeriod;
        }

        public void Update(TaskParams taskParams)
        {
            _id = taskParams.id;
            _userId = taskParams.userId;
            _parentId = taskParams.parentId;
            _name = taskParams.name;
            _description = taskParams.description;
            _tags = taskParams.tags;
            _startTime = taskParams.startTime;
            _deadline = taskParams.deadline;
            _isRepeated = taskParams.isRepeated;
            _repeatPeriod = taskParams.repeatPeriod;
        }

        public void AddTag(int tagId)
        {
            foreach (var node in Tags)
            {
                if (node == tagId)
                    return;
            }
            _tags.Add(tagId);
        }

        public void RemoveTag(int id)
        {
            foreach (var node in Tags)
            {
                if (node == id)
                {
                    Tags.Remove(node);
                    return;
                }
            }
        }

        public void ClearTags()
        {
            Tags.Clear();
        }

        public void AddParent(int parentId)
        {
            _parentId = parentId;
        }

        public void RemoveParent()
        {
            _parentId = Id;
        }
    }
}
