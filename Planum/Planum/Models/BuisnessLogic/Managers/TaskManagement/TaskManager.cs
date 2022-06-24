using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.DTO;
using Serilog;
using Task = Planum.Models.BuisnessLogic.Entities.Task;

namespace Planum.Models.BuisnessLogic.Managers
{
    public class TaskManager : ITaskManager
    {
        protected ITaskRepo _taskRepo;
        protected ITaskConverter _taskConverter;
        protected IUserManager _userManager;

        public TaskManager(ITaskRepo taskRepo, ITaskConverter taskConverter, IUserManager userManager)
        {
            _taskRepo = taskRepo;
            _taskConverter = taskConverter;
            _userManager = userManager;
        }

        public int CreateTask(DateTime startTime, DateTime deadline,
            TimeSpan repeatPeriod, IReadOnlyList<int> TagIds, IReadOnlyList<int> ParentIds, IReadOnlyList<int> ChildIds,
            string name, bool timed = false,
            string description = "", bool isRepeated = false, IReadOnlyList<int> StatusQueueIds = null)
        {
            Log.Debug("Create task");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't create task while current user is null");
            Task new_task = new Task(-1, startTime, deadline, repeatPeriod, TagIds, ParentIds, ChildIds,
                name, timed, _userManager.CurrentUser.Id, description, isRepeated, false, StatusQueueIds);
            new_task.SetStatusIndex(0);
            TaskDTO taskDTO = _taskConverter.ConvertToDTO(new_task);
            int newTaskId = _taskRepo.AddTask(taskDTO);
            foreach (int taskId in new_task.ParentIds)
            {
                AddChildToTask(taskId, newTaskId);
            }

            foreach (int taskId in new_task.ChildIds)
            {
                AddParentToTask(taskId, newTaskId);
            }
            return newTaskId;
        }

        public void UpdateTask(Task task)
        {
            Log.Debug($"Update task with id={task.Id}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't update task while current user is null");
            if (task.UserId != _userManager.CurrentUser.Id)
                throw new TaskDoesNotExistException();

            if (FindTask(task.Id, null) == null) return;

            foreach (int taskId in task.ParentIds)
            {
                RemoveChildFromTask(taskId, task.Id);
            }

            foreach (int taskId in task.ChildIds)
            {
                RemoveParentFromTask(taskId, task.Id);
            }

            foreach(int taskId in task.ParentIds)
            {
                AddChildToTask(taskId, task.Id);
            }

            foreach(int taskId in task.ChildIds)
            {
                AddParentToTask(taskId, task.Id);
            }

            TaskDTO taskDTO = _taskConverter.ConvertToDTO(task);
            _taskRepo.UpdateTask(taskDTO);
        }

        public void DeleteTask(int taskId)
        {
            Log.Debug($"delete task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("can't delete task while current user is null");

            Task? deletedTask = FindTask(taskId, null);

            if (deletedTask == null) return;
            List<int> addedParents = (List<int>)deletedTask.ParentIds;
            foreach(int id in deletedTask.ChildIds)
            {
                Task task = FindTask(id);
                if (task != null)
                {
                    List<int> newParentList = (List<int>)task.ParentIds;
                    newParentList.AddRange(addedParents);
                    UpdateTask(task);
                }
            }

            List<int> addedChildren = (List<int>)deletedTask.ChildIds;
            foreach (int id in deletedTask.ParentIds)
            {
                Task task = FindTask(id);
                if (task != null)
                {
                    List<int> newChildList = (List<int>)task.ChildIds;
                    newChildList.AddRange(addedChildren);
                    UpdateTask(task);
                }
            }

            _taskRepo.DeleteTask(taskId);
        }

        public void ArchiveTask(int taskId)
        {
            Log.Debug($"Archive task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't archive task while current user is null");
            Task? deletedTask = FindTask(taskId);
            if (deletedTask == null) return;
            List<int> addedParents = (List<int>)deletedTask.ParentIds;
            foreach (int id in deletedTask.ChildIds)
            {
                Task task = GetTask(id);
                List<int> newParentList = (List<int>)task.ParentIds;
                newParentList.AddRange(addedParents);
                newParentList.Remove(taskId);
                UpdateTask(task);
            }

            List<int> addedChildren = (List<int>)deletedTask.ChildIds;
            foreach (int id in deletedTask.ParentIds)
            {
                Task task = GetTask(id);
                List<int> newChildList = (List<int>)task.ChildIds;
                newChildList.AddRange(addedChildren);
                newChildList.Remove(taskId);
                UpdateTask(task);
            }
            deletedTask.Archived = true;
            _taskRepo.UpdateTask(_taskConverter.ConvertToDTO(deletedTask));
        }

        public void UnarchiveTask(int taskId)
        {
            Log.Debug($"Unarchive task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't unarchive task while current user is null");
            Task? archivedTask = FindTask(taskId, true);
            if (archivedTask == null) return;
            foreach (int id in archivedTask.ChildIds)
            {
                Task task = GetTask(id);
                List<int> newParentList = (List<int>)task.ParentIds;
                newParentList.Add(taskId);
                UpdateTask(task);
            }

            foreach (int id in archivedTask.ParentIds)
            {
                Task task = GetTask(id);
                List<int> newChildList = (List<int>)task.ChildIds;
                newChildList.Add(archivedTask.Id);
                UpdateTask(task);
            }

            archivedTask.Archived = false;
            _taskRepo.UpdateTask(_taskConverter.ConvertToDTO(archivedTask));
        }

        public Task GetTask(int taskId, bool? archived = false)
        {
            Log.Debug($"Get task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't get task while current user is null");
            TaskDTO task = _taskRepo.GetTask(taskId);
            if (task.UserId != _userManager.CurrentUser.Id)
                throw new IncorrectUserException("Task has incorrect user id");
            if (archived == null)
                return _taskConverter.ConvertFromDTO(_taskRepo.GetTask(taskId));
            else if (task.Archived == archived)
                return _taskConverter.ConvertFromDTO(_taskRepo.GetTask(taskId));
            throw new TaskDoesNotExistException();
        }

        public Task? FindTask(int taskId, bool? archived = false)
        {
            Log.Debug($"Find task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't find task while current user is null");
            TaskDTO? task = _taskRepo.FindTask(taskId);
            if (task == null)
                return null;
            if (task.UserId != _userManager.CurrentUser.Id)
                throw new IncorrectUserException("Task has incorrect user id");
            if (archived == null)
                return _taskConverter.ConvertFromDTO(_taskRepo.GetTask(taskId));
            else if (task.Archived == archived)
                return _taskConverter.ConvertFromDTO(_taskRepo.GetTask(taskId));
            return null;
        }

        public void DeleteConnectedToUser(int userId)
        {
            Log.Debug($"Delete tasks connected to user with id={userId}");
            List<Task> tasks = GetAllTasks(null);
            foreach (Task task in tasks)
            {
                DeleteTask(task.Id);
            }
        }

        public List<Task> GetAllTasks(bool? archived = false)
        {
            Log.Debug("Get all tasks");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't get all tasks while current user is null");
            List<Task> tasks = new List<Task>();
            List<TaskDTO> taskDTOs = _taskRepo.GetAllTasks();

            if (archived == null)
            {
                foreach (var taskDTO in taskDTOs)
                {
                    if (taskDTO.UserId == _userManager.CurrentUser.Id)
                        tasks.Add(_taskConverter.ConvertFromDTO(taskDTO));
                }
            }
            else
            {
                foreach (var taskDTO in taskDTOs)
                {
                    if (taskDTO.Archived == archived && taskDTO.UserId == _userManager.CurrentUser.Id)
                        tasks.Add(_taskConverter.ConvertFromDTO(taskDTO));
                }
            }
            return tasks;
        }

        public void AddTagToTask(int taskId, int tagId)
        {
            Log.Debug($"Add tag id={tagId} to task id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't add tag to task while current user is null");
            Task? task = FindTask(taskId, null);
            if (task == null) return;
            task.AddTag(tagId);
            UpdateTask(task);
        }

        public void RemoveTagFromAll(int tagId)
        {
            Log.Debug($"Remove tag with id={tagId} from all tasks");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't remove tag from all tasks while current user is null");
            List<Task> tasks = GetAllTasks(null);
            foreach (Task task in tasks)
            {
                task.RemoveTag(tagId);
                UpdateTask(task);
            }
        }

        public void RemoveTagFromTask(int taskId, int tagId)
        {
            Log.Debug($"Remove tag with id={tagId} from all task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't remove tag from task while current user is null");
            Task? task = FindTask(taskId, null);
            if (task == null) return;
            task.RemoveTag(tagId);
            UpdateTask(task);
        }

        public void AddChildToTask(int taskId, int childId)
        {
            Log.Debug($"Add child with id={childId} to task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't add child to task while current user is null");
            Task? task = FindTask(taskId, null);
            if (task == null) return;
            Task? child = FindTask(childId, null);
            if (child == null) return;
            task.AddChild(childId);
            TaskDTO temp = _taskConverter.ConvertToDTO(task);
            _taskRepo.UpdateTask(temp);
            child.AddParent(taskId);
            temp = _taskConverter.ConvertToDTO(child);
            _taskRepo.UpdateTask(temp);
        }

        public void RemoveChildFromTask(int taskId, int childId)
        {
            Log.Debug($"Remove child with id={childId} to task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't remove child from task while current user is null");
            Task? task = FindTask(taskId, null);
            if (task == null) return;
            Task? child = FindTask(childId, null);
            if (child == null) return;
            task.RemoveChild(childId);
            TaskDTO temp = _taskConverter.ConvertToDTO(task);
            _taskRepo.UpdateTask(temp);
            child.RemoveParent(taskId);
            temp = _taskConverter.ConvertToDTO(child);
            _taskRepo.UpdateTask(temp);
        }

        public void AddParentToTask(int taskId, int parentId)
        {
            Log.Debug($"Add parent with id={parentId} to task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't add parent to task while current user is null");
            Task? task = FindTask(taskId, null);
            if (task == null) return;
            Task? parent = FindTask(parentId, null);
            if (parent == null) return;
            task.AddParent(parentId);
            TaskDTO temp = _taskConverter.ConvertToDTO(task);
            _taskRepo.UpdateTask(temp);
            parent.AddChild(taskId);
            temp = _taskConverter.ConvertToDTO(parent);
            _taskRepo.UpdateTask(temp);
        }

        public void RemoveParentFromTask(int taskId, int parentId)
        {
            Log.Debug($"Remove child with id={parentId} to task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't remove parent from task while current user is null");
            Task? task = FindTask(taskId, null);
            if (task == null) return;
            Task? parent = FindTask(parentId, null);
            if (parent == null) return;
            task.RemoveParent(parentId);
            TaskDTO temp = _taskConverter.ConvertToDTO(task);
            _taskRepo.UpdateTask(temp);
            parent.RemoveChild(taskId);
            temp = _taskConverter.ConvertToDTO(parent);
            _taskRepo.UpdateTask(temp);
        }

        public void ClearTags(int taskId)
        {
            Log.Debug($"Clear tags from task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't clear tags while current user is null");
            Task? task = FindTask(taskId, null);
            if (task == null) return;
            task.ClearTags();
            UpdateTask(task);
        }

        public void ClearChildren(int taskId)
        {
            Log.Debug($"Clear children from task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't clear children while current user is null");
            Task? task = FindTask(taskId, null);
            if (task == null) return;
            foreach (int childId in task.ChildIds)
            {
                Task temp = GetTask(childId, null);
                temp.RemoveParent(taskId);
                UpdateTask(task);
            }
            task.ClearChildIds();
            UpdateTask(task);
        }

        public void ClearParents(int taskId)
        {
            Log.Debug($"Clear parents from task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't clear parents while current user is null");
            Task? task = FindTask(taskId, null);
            if (task == null) return;
            foreach (int parentId in task.ParentIds)
            {
                Task temp = GetTask(parentId, null);
                temp.RemoveChild(taskId);
                UpdateTask(task);
            }
            task.ClearParents();
            UpdateTask(task);
        }

        public bool TaskHasStatus(int taskId)
        {
            Log.Debug($"Check if task with id={taskId} has status");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't check task status while current user is null");
            Task? task = FindTask(taskId);
            if (task == null)
                throw new TaskDoesNotExistException();
            if (task.StatusQueueIds.Count > 0)
                return true;
            else
                return false;
        }

        public void AddStatusToTask(int statusId, int taskId, ITagManager tagManager)
        {
            Log.Debug($"Add status id={statusId} to task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't add status to task while current user is null");
            Task? task = FindTask(taskId);
            if (task == null) return;
            if (tagManager.FindTag(statusId) == null) return;
            task.AddStatus(statusId);
            UpdateTask(task);
        }

        public void RemoveStatus(int statusId, int taskId)
        {
            Log.Debug($"Remove status id={statusId} in task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't remove status from task while current user is null");
            Task? task = FindTask(taskId);
            if (task == null) return;
            task.RemoveStatus(statusId);
            UpdateTask(task);
        }

        public void RemoveStatusAt(int position, int taskId)
        {
            Log.Debug($"Remove status at position={position} in task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't remove status at position while current user is null");
            Task? task = FindTask(taskId);
            if (task == null) return;
            task.RemoveStatusAtPosition(position);
            UpdateTask(task);
        }

        public void NextStatus(int taskId)
        {
            Log.Debug($"Next status for task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't change task status while current user is null");
            Task? task = FindTask(taskId);
            if (task == null) return;
            task.NextStatus();
            UpdateTask(task);
        }

        public void PreviousStatus(int taskId)
        {
            Log.Debug($"Next status for task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't change task status while current user is null");
            Task? task = FindTask(taskId);
            if (task == null) return;
            task.PreviousStatus();
            UpdateTask(task);
        }

        public void CompleteTask(int taskId)
        {
            Log.Debug($"Complete task with id={taskId}");
            if (_userManager.CurrentUser == null)
                throw new CurrentUserIsNullException("Can't change task status while current user is null");
            Task? task = FindTask(taskId);
            if (task == null) return;
            if (task.IsRepeated)
            {
                DateTime startTime = task.StartTime;
                DateTime deadline = task.Deadline;
                if (task.StartTime != DateTime.MinValue)
                    startTime = startTime.Add(task.RepeatPeriod);
                if (task.Deadline != DateTime.MinValue)
                    deadline = deadline.Add(task.RepeatPeriod);
                Task newTask = new Task(task.Id, startTime, deadline, task.RepeatPeriod, task.TagIds, task.ParentIds,
                    task.ChildIds, task.Name, task.Timed, task.UserId, task.Description, task.IsRepeated, task.Archived,
                    task.StatusQueueIds);

                newTask.SetStatusIndex(0);
                UpdateTask(newTask);
            }
            else
                ArchiveTask(taskId);
        }
    }
}
