using System;
using System.Collections.Generic;
using Planum.Models.BuisnessLayer.Managers.TaskManagement;
using Planum.Models.BuisnessLayer.RepoInterfaces;
using Planum.Models.DTO;
using Task = Planum.Models.BuisnessLayer.Entities.Task;

namespace Planum.Models.BuisnessLayer.Managers
{
    public class TaskManager : ITaskManager
    {
        protected ITaskRepo _taskRepo;
        protected ITaskConverter _taskConverter;

        public TaskManager(ITaskRepo taskRepo, ITaskConverter taskConverter)
        {
            _taskRepo = taskRepo;
            _taskConverter = taskConverter;
        }

        public int CreateTask(int user_id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, List<int> tagIds, bool timed = false, string name = "",
            string description = "", int parentId = -1, bool isRepeated = false)
        {
            Task new_task = new Task(-1, startTime, deadline, repeatPeriod, tagIds, timed, user_id, name, description, parentId, isRepeated);
            TaskDTO taskDTO = _taskConverter.ConvertToDTO(new_task);
            return _taskRepo.AddTask(taskDTO);
        }

        public void UpdateTask(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> tagIds, bool timed = false,
            string name = "", string description = "",
            int parentId = -1, bool isRepeated = false)
        {
            Task? task = FindTask(id);

            if (task == null) return;

            TaskDTO taskDTO = new TaskDTO(id, startTime,
                deadline, repeatPeriod, tagIds, timed, task.UserId, name, description, parentId, isRepeated);
            _taskRepo.UpdateTask(taskDTO);
        }

        public void DeleteTask(int taskId)
        {
            if (FindTask(taskId) != null)
                _taskRepo.DeleteTask(taskId);
        }

        public void ArchiveTask(int taskId)
        {
            if (FindTask(taskId) != null)
                _taskRepo.ArchiveTask(taskId);
        }

        public void UnarchiveTask(int taskId)
        {
            if (FindTask(taskId) != null)
                _taskRepo.UnarchiveTask(taskId);
        }

        public Task GetTask(int taskId)
        {
            return _taskConverter.ConvertFromDTO(_taskRepo.GetTask(taskId));
        }

        public Task? FindTask(int taskId)
        {
            TaskDTO? taskDTO = _taskRepo.FindTask(taskId);
            if (taskDTO == null)
                return null;
            return _taskConverter.ConvertFromDTO(taskDTO);
        }

        public void RemoveTagFromAll(int tagId)
        {
            List<Task> tasks = GetAllTasks();
            foreach (Task task in tasks)
            {
                task.RemoveTag(tagId);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, new List<int>(task.TagIds),
                    task.Timed, task.Name, task.Description, task.ParentId, task.IsRepeated);
            }
        }

        public void DeleteConnectedToUser(int userId)
        {
            List<Task> tasks = GetAllTasks();
            foreach (Task task in tasks)
            {
                if (task.UserId == userId)
                    DeleteTask(task.Id);
            }
        }

        public List<Task> GetAllTasks()
        {
            List<Task> tasks = new List<Task>();
            List<TaskDTO> taskDTOs = _taskRepo.GetAllTasks();

            foreach (var taskDTO in taskDTOs)
            {
                tasks.Add(_taskConverter.ConvertFromDTO(taskDTO));
            }
            return tasks;
        }
    }
}
