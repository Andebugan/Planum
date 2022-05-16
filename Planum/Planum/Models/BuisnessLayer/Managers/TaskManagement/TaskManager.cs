using System;
using System.Collections.Generic;
using Planum.Models.BuisnessLayer.Managers.TaskManagement;
using Planum.Models.BuisnessLayer.RepoInterfaces;
using Planum.Models.DTO.ModelData;
using Task = Planum.Models.BuisnessLayer.Entities.Task;

namespace Planum.Models.BuisnessLayer.Managers
{
    public class TaskManager
    {
        protected ITaskRepo _taskRepo;
        protected ITaskConverter _taskConverter;

        public TaskManager(ITaskRepo taskRepo, ITaskConverter taskConverter)
        {
            _taskRepo = taskRepo;
            _taskConverter = taskConverter;
        }

        public void CreateTask(int user_id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, List<int> tagIds, string name = "", string description = "", int parentId = -1, bool isRepeated = false)
        {
            List<Task> tasks = GetAll();

            // id in repo

            Task new_task = new Task(-1, startTime, deadline, repeatPeriod, tagIds, user_id, name, description, parentId, isRepeated);
            TaskDTO taskDTO = _taskConverter.ConvertToDTO(new_task);
            _taskRepo.Add(taskDTO);
        }

        public void UpdateTask(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> tagIds, string name = "", string description = "",
            int parentId = -1, bool isRepeated = false)
        {
            Task task = GetTask(id);

            TaskDTO taskDTO = new TaskDTO(id, startTime, 
                deadline, repeatPeriod, tagIds, task.UserId, name, description, parentId, isRepeated);
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
            return _taskConverter.ConvertFromDTO(_taskRepo.Get(taskId));
        }

        public void RemoveTagFromAll(int tagId)
        {
            List<Task> tasks = GetAll();
            foreach (Task task in tasks)
            {
                task.RemoveTag(tagId);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, new List<int>(task.TagIds),
                    task.Name, task.Description, task.ParentId, task.IsRepeated);
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
                tasks.Add(_taskConverter.ConvertFromDTO(taskDTO));
            }
            return tasks;
        }
    }
}
