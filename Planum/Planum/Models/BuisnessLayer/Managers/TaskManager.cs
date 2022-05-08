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
    internal class TaskManager
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
            TaskParams taskParams = new TaskParams();

            taskParams.id = taskDTO.Id;
            taskParams.userId = taskDTO.UserId;
            taskParams.parentId = taskDTO.ParentId;
            taskParams.name = taskDTO.Name;
            taskParams.description = taskDTO.Description;
            taskParams.tags = taskDTO.Tags;
            taskParams.startTime = taskDTO.StartTime;
            taskParams.deadline = taskDTO.Deadline;
            taskParams.repeatPeriod = taskDTO.RepeatPeriod;
            taskParams.isRepeated = taskDTO.IsRepeated;

            return new Task(taskParams);
        }

        protected TaskDTO ConvertToDTO(Task task)
        {
            TaskParamsDTO taskParamsDTO = new TaskParamsDTO();

            taskParamsDTO.id = task.Id;
            taskParamsDTO.userId = task.UserId;
            taskParamsDTO.parentId = task.ParentId;
            taskParamsDTO.name = task.Name;
            taskParamsDTO.description = task.Description;
            taskParamsDTO.tags = task.Tags;
            taskParamsDTO.startTime = task.StartTime;
            taskParamsDTO.deadline = task.Deadline;
            taskParamsDTO.repeatPeriod = task.RepeatPeriod;
            taskParamsDTO.isRepeated = task.IsRepeated;

            return new TaskDTO(taskParamsDTO);
        }

        public void CreateTask(TaskParams taskParams)
        {
            Task task = new Task(taskParams);
            TaskDTO taskDTO = ConvertToDTO(task);
            _taskRepo.Add(taskDTO);
        }

        public void UpdateTask(ref Task task, TaskParams taskParams)
        {
            task.Update(taskParams);
            TaskDTO taskDTO = ConvertToDTO(task);
            _taskRepo.Update(taskDTO);
        }

        public void UpdateTask(ref Task task)
        {
            TaskDTO taskDTO = ConvertToDTO(task);
            _taskRepo.Update(taskDTO);
        }

        public void UpdateTask(Task task)
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

        public Task? GetTask(int taskId)
        {
            return ConvertFromDTO(_taskRepo.Get(taskId));
        }

        public void RemoveTagFromAll(int tagId)
        {
            List<Task> tasks = GetAll();
            foreach (Task task in tasks)
            {
                task.RemoveTag(tagId);
                UpdateTask(task);
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
