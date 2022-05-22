using System;
using System.Collections.Generic;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.DTO;
using Task = Planum.Models.BuisnessLogic.Entities.Task;

namespace Planum.Models.BuisnessLogic.Managers
{
    public class TaskManager : ITaskManager
    {
        protected ITaskRepo _taskRepo;
        protected ITaskConverter _taskConverter;
        protected ITagManager _tagManager;

        public TaskManager(ITaskRepo taskRepo, ITaskConverter taskConverter)
        {
            _taskRepo = taskRepo;
            _taskConverter = taskConverter;
        }

        public int CreateTask(DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, int userId, bool timed = false,
            string description = "", bool isRepeated = false)
        {
            Task new_task = new Task(-1, startTime, deadline, repeatPeriod, TagIds, ParentIds, ChildIds,
                name, timed, userId, description, isRepeated);
            TaskDTO taskDTO = _taskConverter.ConvertToDTO(new_task);
            return _taskRepo.AddTask(taskDTO);
        }

        public void UpdateTask(int id, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, string description = "", bool isRepeated = false)
        {
            Task? task = FindTask(id);

            if (task == null) return;

            TaskDTO taskDTO = new TaskDTO(id, startTime,
                deadline, repeatPeriod, TagIds, ParentIds, ChildIds, name, timed, task.UserId, description, isRepeated);
            _taskRepo.UpdateTask(taskDTO);
        }

        public void UpdateTask(int id, int userId, DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false, string description = "", bool isRepeated = false)
        {
            Task? task = FindTask(id, userId);

            if (task == null) return;

            TaskDTO taskDTO = new TaskDTO(id, startTime,
                deadline, repeatPeriod, TagIds, ParentIds, ChildIds, name, timed, task.UserId, description, isRepeated);
            _taskRepo.UpdateTask(taskDTO);
        }

        public void DeleteTask(int taskId)
        {
            Task? deletedTask = FindTask(taskId);
            if (deletedTask == null) return;
            List<int> addedParents = (List<int>)deletedTask.ParentIds;
            foreach(int id in deletedTask.ChildIds)
            {
                Task task = GetTask(id);
                List<int> newParentList = (List<int>)task.ParentIds;
                newParentList.AddRange(addedParents);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, newParentList, task.ChildIds,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }

            List<int> addedChildren = (List<int>)deletedTask.ChildIds;
            foreach (int id in deletedTask.ParentIds)
            {
                Task task = GetTask(id);
                List<int> newChildList = (List<int>)task.ChildIds;
                newChildList.AddRange(addedChildren);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, task.ParentIds, newChildList,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }

            _taskRepo.DeleteTask(taskId);
        }

        public void DeleteTask(int taskId, int userId)
        {
            Task? deletedTask = FindTask(taskId, userId);
            if (deletedTask == null) return;
            List<int> addedParents = (List<int>)deletedTask.ParentIds;
            foreach (int id in deletedTask.ChildIds)
            {
                Task task = GetTask(id);
                List<int> newParentList = (List<int>)task.ParentIds;
                newParentList.AddRange(addedParents);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, newParentList, task.ChildIds,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }

            List<int> addedChildren = (List<int>)deletedTask.ChildIds;
            foreach (int id in deletedTask.ParentIds)
            {
                Task task = GetTask(id);
                List<int> newChildList = (List<int>)task.ChildIds;
                newChildList.AddRange(addedChildren);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, task.ParentIds, newChildList,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }

            _taskRepo.DeleteTask(taskId);
        }

        public void ArchiveTask(int taskId)
        {
            Task? deletedTask = FindTask(taskId);
            if (deletedTask == null) return;
            List<int> addedParents = (List<int>)deletedTask.ParentIds;
            foreach (int id in deletedTask.ChildIds)
            {
                Task task = GetTask(id);
                List<int> newParentList = (List<int>)task.ParentIds;
                newParentList.AddRange(addedParents);
                newParentList.Remove(taskId);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, newParentList, task.ChildIds,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }

            List<int> addedChildren = (List<int>)deletedTask.ChildIds;
            foreach (int id in deletedTask.ParentIds)
            {
                Task task = GetTask(id);
                List<int> newChildList = (List<int>)task.ChildIds;
                newChildList.AddRange(addedChildren);
                newChildList.Remove(taskId);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, task.ParentIds, newChildList,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }
            _taskRepo.ArchiveTask(taskId);
        }

        public void ArchiveTask(int taskId, int userId)
        {
            Task? deletedTask = FindTask(taskId, userId);
            if (deletedTask == null) return;
            List<int> addedParents = (List<int>)deletedTask.ParentIds;
            foreach (int id in deletedTask.ChildIds)
            {
                Task task = GetTask(id);
                List<int> newParentList = (List<int>)task.ParentIds;
                newParentList.AddRange(addedParents);
                newParentList.Remove(taskId);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, newParentList, task.ChildIds,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }

            List<int> addedChildren = (List<int>)deletedTask.ChildIds;
            foreach (int id in deletedTask.ParentIds)
            {
                Task task = GetTask(id);
                List<int> newChildList = (List<int>)task.ChildIds;
                newChildList.AddRange(addedChildren);
                newChildList.Remove(taskId);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, task.ParentIds, newChildList,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }
            _taskRepo.ArchiveTask(taskId);
        }

        public void UnarchiveTask(int taskId)
        {
            Task? archivedTask = FindArchivedTask(taskId);
            if (archivedTask == null) return;
            foreach (int id in archivedTask.ChildIds)
            {
                Task task = GetTask(id);
                List<int> newParentList = (List<int>)task.ParentIds;
                newParentList.Add(taskId);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, newParentList, task.ChildIds,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }

            foreach (int id in archivedTask.ParentIds)
            {
                Task task = GetTask(id);
                List<int> newChildList = (List<int>)task.ChildIds;
                newChildList.Add(archivedTask.Id);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, task.ParentIds, newChildList,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }

            _taskRepo.UnarchiveTask(taskId);
        }

        public void UnarchiveTask(int taskId, int userId)
        {
            Task? archivedTask = FindArchivedTask(taskId, userId);
            if (archivedTask == null) return;
            foreach (int id in archivedTask.ChildIds)
            {
                Task task = GetTask(id);
                List<int> newParentList = (List<int>)task.ParentIds;
                newParentList.Add(taskId);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, newParentList, task.ChildIds,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }

            foreach (int id in archivedTask.ParentIds)
            {
                Task task = GetTask(id);
                List<int> newChildList = (List<int>)task.ChildIds;
                newChildList.Add(archivedTask.Id);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds, task.ParentIds, newChildList,
                    task.Name, task.Timed, task.Description, task.IsRepeated);
            }

            _taskRepo.UnarchiveTask(taskId);
        }

        public Task GetTask(int taskId)
        {
            return _taskConverter.ConvertFromDTO(_taskRepo.GetTask(taskId));
        }

        public Task GetTask(int taskId, int userId)
        {
            return _taskConverter.ConvertFromDTO(_taskRepo.GetTask(taskId, userId));
        }

        public Task GetArhcivedTask(int taskId)
        {
            return _taskConverter.ConvertFromDTO(_taskRepo.GetArchivedTask(taskId));
        }

        public Task GetArhcivedTask(int taskId, int userId)
        {
            return _taskConverter.ConvertFromDTO(_taskRepo.GetArchivedTask(taskId, userId));
        }


        public Task? FindTask(int taskId)
        {
            TaskDTO? taskDTO = _taskRepo.FindTask(taskId);
            if (taskDTO == null)
                return null;
            return _taskConverter.ConvertFromDTO(taskDTO);
        }

        public Task? FindTask(int taskId, int userId)
        {
            TaskDTO? taskDTO = _taskRepo.FindTask(taskId, userId);
            if (taskDTO == null)
                return null;
            return _taskConverter.ConvertFromDTO(taskDTO);
        }

        public Task? FindArchivedTask(int taskId)
        {
            TaskDTO? taskDTO = _taskRepo.FindArchivedTask(taskId);
            if (taskDTO == null)
                return null;
            return _taskConverter.ConvertFromDTO(taskDTO);
        }

        public Task? FindArchivedTask(int taskId, int userId)
        {
            TaskDTO? taskDTO = _taskRepo.FindArchivedTask(taskId, userId);
            if (taskDTO == null)
                return null;
            return _taskConverter.ConvertFromDTO(taskDTO);
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

        public List<Task> GetAllTasks(int userId)
        {
            List<Task> tasks = new List<Task>();
            List<TaskDTO> taskDTOs = _taskRepo.GetAllTasks();

            foreach (var taskDTO in taskDTOs)
            {
                if (taskDTO.UserId == userId)
                    tasks.Add(_taskConverter.ConvertFromDTO(taskDTO));
            }
            return tasks;
        }

        public void AddTagToTask(int taskId, int tagId)
        {
            Task? task = FindTask(taskId);
            if (task == null) return;
            task.AddTag(tagId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
        }

        public void AddTagToTask(int taskId, int tagId, int userId)
        {
            Task? task = FindTask(taskId, userId);
            if (task == null) return;
            task.AddTag(tagId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
        }

        public void RemoveTagFromAll(int tagId)
        {
            List<Task> tasks = GetAllTasks();
            foreach (Task task in tasks)
            {
                task.RemoveTag(tagId);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
            }
        }

        public void RemoveTagFromAll(int tagId, int userId)
        {
            List<Task> tasks = GetAllTasks(userId);
            foreach (Task task in tasks)
            {
                task.RemoveTag(tagId);
                UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
            }
        }

        public void RemoveTagFromTask(int taskId, int tagId)
        {
            Task? task = FindTask(taskId);
            if (task == null) return;
            task.RemoveTag(tagId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
        }

        public void RemoveTagFromTask(int taskId, int tagId, int userId)
        {
            Task? task = FindTask(taskId, userId);
            if (task == null) return;
            task.RemoveTag(tagId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
        }

        public void AddChildToTask(int taskId, int childId)
        {
            Task? task = FindTask(taskId);
            if (task == null) return;
            Task? child = FindTask(childId);
            if (child == null) return;
            task.AddChild(childId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
            child.AddParent(taskId);
            UpdateTask(child.Id, child.StartTime, child.Deadline, child.RepeatPeriod, child.TagIds,
                child.ParentIds, child.ChildIds, child.Name, child.Timed, child.Description, child.IsRepeated);
        }

        public void AddChildToTask(int taskId, int childId, int userId)
        {
            Task? task = FindTask(taskId, userId);
            if (task == null) return;
            Task? child = FindTask(childId, userId);
            if (child == null) return;
            task.AddChild(childId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
            child.AddParent(taskId);
            UpdateTask(child.Id, child.StartTime, child.Deadline, child.RepeatPeriod, child.TagIds,
                child.ParentIds, child.ChildIds, child.Name, child.Timed, child.Description, child.IsRepeated);
        }

        public void RemoveChildFromTask(int taskId, int childId)
        {
            Task? task = FindTask(taskId);
            if (task == null) return;
            Task? child = FindTask(childId);
            if (child == null) return;
            task.RemoveChild(childId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
            child.RemoveParent(taskId);
            UpdateTask(child.Id, child.StartTime, child.Deadline, child.RepeatPeriod, child.TagIds,
                child.ParentIds, child.ChildIds, child.Name, child.Timed, child.Description, child.IsRepeated);
        }

        public void RemoveChildFromTask(int taskId, int childId, int userId)
        {
            Task? task = FindTask(taskId, userId);
            if (task == null) return;
            Task? child = FindTask(childId, userId);
            if (child == null) return;
            task.RemoveChild(childId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
            child.RemoveParent(taskId);
            UpdateTask(child.Id, child.StartTime, child.Deadline, child.RepeatPeriod, child.TagIds,
                child.ParentIds, child.ChildIds, child.Name, child.Timed, child.Description, child.IsRepeated);
        }

        public void AddParentToTask(int taskId, int parentId)
        {
            Task? task = FindTask(taskId);
            if (task == null) return;
            Task? parent = FindTask(parentId);
            if (parent == null) return;
            task.AddParent(parentId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
            parent.AddChild(taskId);
            UpdateTask(parent.Id, parent.StartTime, parent.Deadline, parent.RepeatPeriod, parent.TagIds,
                    parent.ParentIds, parent.ChildIds, parent.Name, parent.Timed, parent.Description, parent.IsRepeated);
        }

        public void AddParentToTask(int taskId, int parentId, int userId)
        {
            Task? task = FindTask(taskId, userId);
            if (task == null) return;
            Task? parent = FindTask(parentId, userId);
            if (parent == null) return;
            task.AddParent(parentId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
            parent.AddChild(taskId);
            UpdateTask(parent.Id, parent.StartTime, parent.Deadline, parent.RepeatPeriod, parent.TagIds,
                    parent.ParentIds, parent.ChildIds, parent.Name, parent.Timed, parent.Description, parent.IsRepeated);
        }

        public void RemoveParentFromTask(int taskId, int parentId)
        {
            Task? task = FindTask(taskId);
            if (task == null) return;
            Task? parent = FindTask(parentId);
            if (parent == null) return;
            task.RemoveParent(parentId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
            parent.RemoveChild(taskId);
            UpdateTask(parent.Id, parent.StartTime, parent.Deadline, parent.RepeatPeriod, parent.TagIds,
                    parent.ParentIds, parent.ChildIds, parent.Name, parent.Timed, parent.Description, parent.IsRepeated);
        }

        public void RemoveParentFromTask(int taskId, int parentId, int userId)
        {
            Task? task = FindTask(taskId, userId);
            if (task == null) return;
            Task? parent = FindTask(parentId, userId);
            if (parent == null) return;
            task.RemoveParent(parentId);
            UpdateTask(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, task.TagIds,
                    task.ParentIds, task.ChildIds, task.Name, task.Timed, task.Description, task.IsRepeated);
            parent.RemoveChild(taskId);
            UpdateTask(parent.Id, parent.StartTime, parent.Deadline, parent.RepeatPeriod, parent.TagIds,
                    parent.ParentIds, parent.ChildIds, parent.Name, parent.Timed, parent.Description, parent.IsRepeated);
        }
    }
}
