using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.BuisnessLayer.Interfaces;
using Planum.Models.DTO.ModelData;
using Task = Planum.Models.BuisnessLayer.Entities.Task;

namespace Planum.Models.BuisnessLayer.Managers
{
    public class TaskManager
    {
        protected ITaskRepo _taskRepo;

        public TaskManager(ref ITaskRepo taskRepo)
        {
            if (taskRepo == null)
                throw new ArgumentNullException(nameof(taskRepo));
            _taskRepo = taskRepo;
        }

        protected Task ConvertFromDTO(TaskDTO taskDTO)
        {
            return new Task(taskDTO.Id, taskDTO.UserId, taskDTO.Name, taskDTO.StartTime, taskDTO.Deadline, taskDTO.RepeatPeriod,
                taskDTO.TagIds, taskDTO.Description, taskDTO.ParentId, taskDTO.IsRepeated);
        }

        protected TaskDTO ConvertToDTO(Task task)
        {
            return new TaskDTO(task.Id, task.UserId, task.Name, task.StartTime, task.Deadline, task.RepeatPeriod,
                task.TagIds, task.Description, task.ParentId, task.IsRepeated);
        }

        public void CreateTask(int user_id, string? name, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, List<int> tagIds, string? description = null, int parentId = -1, bool isRepeated = false)
        {
            List<Task> tasks = GetAll();

            int id = 0;
            foreach (var task in tasks)
            {
                if (id == task.Id)
                    id += 1;
            }

            Task new_task = new Task(id, user_id, name, startTime, deadline, repeatPeriod, tagIds, description, parentId, isRepeated);
            TaskDTO taskDTO = ConvertToDTO(new_task);
            _taskRepo.Add(taskDTO);
        }

        public void CreateTask(int user_id, string? name, List<int> tagIds,
            string? description = null, int parentId = -1, bool isRepeated = false)
        {
            List<Task> tasks = GetAll();

            int id = 0;
            foreach (var task in tasks)
            {
                if (id == task.Id)
                    id += 1;
            }

            Task new_task = new Task(id, user_id, name, tagIds, description, parentId, isRepeated);
            TaskDTO taskDTO = ConvertToDTO(new_task);
            _taskRepo.Add(taskDTO);
        }

        public void Update(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, List<int> tagIds, string? name, string? description = null,
            int parentId = -1, bool isRepeated = false)
        {
            Task task = GetTask(id);
            if (tagIds == null)
                tagIds = task.TagIds;
            if (name == null)
                name = task.Name;
            if (description == null)
                description = task.Description;
            if (parentId == -1)
                parentId = task.ParentId;
            if (isRepeated == false)
                isRepeated = task.IsRepeated;
            if (Math.Abs((startTime - DateTime.MinValue).TotalSeconds) > 1)
                startTime = task.StartTime;
            if (Math.Abs((deadline - DateTime.MinValue).TotalSeconds) > 1)
                deadline = task.Deadline;
            if (Math.Abs((repeatPeriod - TimeSpan.Zero).TotalSeconds) > 1)
                repeatPeriod = task.RepeatPeriod;

            TaskDTO taskDTO = new TaskDTO(id, task.UserId, name, startTime, 
                deadline, repeatPeriod, tagIds, description, parentId, isRepeated);
            _taskRepo.Update(taskDTO);
        }
        public void Update(int id, List<int> tagIds, string? name = null,
            string? description = null, int parentId = -1, bool isRepeated = false)
        {
            Task task = GetTask(id);
            if (tagIds == null)
                tagIds = task.TagIds;
            if (name == null)
                name = task.Name;
            if (description == null)
                description = task.Description;
            if (parentId == -1)
                parentId = task.ParentId;
            if (isRepeated == false)
                isRepeated = task.IsRepeated;

            TaskDTO taskDTO = new TaskDTO(id, task.UserId, name, description, parentId, isRepeated);
            _taskRepo.Update(taskDTO);
        }

        public void Update(Task task)
        {
            TaskDTO taskDTO = ConvertToDTO(task);
            _taskRepo.Update(taskDTO);
        }


        public void Update(ref Task task)
        {
            TaskDTO taskDTO = ConvertToDTO(task);
            _taskRepo.Update(taskDTO);
        }

        public void DeleteTask(int taskId)
        {
            _taskRepo.Delete(taskId);
        }

        public void ArchiveTask(int taskId)
        {
            _taskRepo.Archive(taskId);
        }

        public void UnarchiveTask(int taskId)
        {
            _taskRepo.Unarchive(taskId);
        }

        public Task GetTask(int taskId)
        {
            return ConvertFromDTO(_taskRepo.Get(taskId));
        }

        public void RemoveTagFromAll(int tagId)
        {
            List<Task> tasks = GetAll();
            foreach (Task task in tasks)
            {
                task.RemoveTag(tagId);
                Update(task);
            }
        }

        public void DeleteConnectedToUser(int userId)
        {
            List<Task> tasks = GetAll();
            foreach (Task task in tasks)
            {
                if (task.UserId == userId)
                    DeleteTask(task.Id);
            }
        }

        public List<Task> GetAll()
        {
            List<Task> tasks = new List<Task>();
            List<TaskDTO> taskDTOs = _taskRepo.GetAll();

            foreach (var taskDTO in taskDTOs)
            {
                tasks.Add(ConvertFromDTO(taskDTO));
            }
            return tasks;
        }
    }
}
