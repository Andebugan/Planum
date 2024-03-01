using Planum.ConsoleUI.UI;
using Planum.Model.Entities;
using Planum.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Planum.Model.Managers
{
    public class TaskManager
    {
        protected Repo saveRepo;

        public TaskManager()
        {
            saveRepo = new Repo();
        }

        public int CreateTask(Task task)
        {
            int newTaskId = saveRepo.Add(task);

            List<Task> tasks = FindTask();

            UpdateRelations(task, newTaskId);
            return newTaskId;
        }

        public void UpdateRelations(Task task, int taskId = -1, bool remove = false)
        {
            if (taskId == -1)
                taskId = task.Id;
            
            // update children
            List<Task> children = saveRepo.Find(task.ChildIds);
            
            foreach (Task child in children)
            {
                if (remove)
                    child.ParentIds.Remove(taskId);
                else if (!child.ParentIds.Contains(taskId))
                    child.ParentIds.Add(taskId);
            }

            saveRepo.Update(children);
            
            // update parents
            List<Task> parents = saveRepo.Find(task.ParentIds);

            foreach (Task parent in parents)
            {
                if (remove)
                    parent.ChildIds.Remove(taskId);
                else if (!parent.ParentIds.Contains(taskId))
                    parent.ChildIds.Add(taskId);
            }

            saveRepo.Update(parents);
        }

        public void RelationNodeUpdate(Task task, bool restore = false)
        {
            List<Task> children = saveRepo.Find(task.ChildIds);
            List<Task> parents = saveRepo.Find(task.ParentIds);

            foreach (Task child in children)
            {
                foreach (Task parent in parents)
                {
                    if (restore)
                    {
                        child.ParentIds.Remove(parent.Id);
                        parent.ChildIds.Remove(child.Id);
                    }
                    else
                    {
                        child.ParentIds.Add(parent.Id);
                        parent.ChildIds.Add(child.Id);
                    }
                }
            }

            saveRepo.Update(parents.Concat(children).ToList());
        }

        public void UpdateTask(Task task)
        {
            Task? oldTask = FindTask(task.Id);
            if (oldTask == null) return;

            UpdateRelations(oldTask, remove: true);
            UpdateRelations(task);

            saveRepo.Update(task);
        }

        public void DeleteTask(List<int> ids)
        {
            List<Task> deletedTasks = FindTask(ids);

            foreach (var task in deletedTasks)
            {
                UpdateRelations(task, remove: true);

                RelationNodeUpdate(task, true);
            }

            saveRepo.Delete(ids);
        }

        public void UpdateArchived(Task archivedTask, bool unarchive = false)
        {
            List<int> taskParents = new List<int>(archivedTask.ParentIds);
            List<int> taskChildren = new List<int>(archivedTask.ChildIds);

            if (unarchive)
            {
                RelationNodeUpdate(archivedTask, restore: true);
                UpdateRelations(archivedTask, remove: false);
                archivedTask.Archived = false;
            }
            else
            {
                RelationNodeUpdate(archivedTask, restore: false);
                UpdateRelations(archivedTask, remove: true);
                archivedTask.Archived = true;
            }
            saveRepo.Update(archivedTask);
        }

        public Task? FindTask(int taskId)
        {
            return saveRepo.Find(taskId);
        }

        public Task? FindTask(string name)
        {
            List<Task> tasks = saveRepo.Find();
            foreach (var task in tasks)
            {
                if (task.Name == name)
                    return task;
            }
            return null;
        }

        public List<Task> FindTask(List<int>? taskIds = null, List<string>? names = null)
        {
            List<Task> tasks = saveRepo.Find();
            List<Task> result = new List<Task>();

            foreach (var task in tasks)
            {
                if (taskIds != null && taskIds.Contains(task.Id))
                    result.Add(task);
                if (names != null && names.Contains(task.Name))
                    result.Add(task);
                if (taskIds == null && names == null)
                    result.Add(task);
            }
            return result.Distinct().ToList();
        }

        public void CheckAutorepeat()
        {
            List<Task> tasks = FindTask();
            foreach (var task in tasks)
            {
                if (task.IsOverdue() && task.Timed() && task.Repeated() && task.TimeParams.repeat.autorepeat)
                {
                    task.ApplyRepeat();

                    UpdateTask(task);
                }
            }
        }

        public void ApplyRepeatPeriod(ref Task task)
        {
            if (task.Repeated())
            {
                task.ApplyRepeat();
            }
        }

        public void CompleteTask(int taskId)
        {
            Task? task = FindTask(taskId);
            if (task == null) return;
            if (task.Repeated())
            {
                task.ApplyRepeat();
                
                UpdateTask(task);
            }
            else
                UpdateArchived(task);
        }

        public void UncompleteTask(int taskId)
        {
            Task? task = FindTask(taskId);
            if (task == null) return;
            if (task.Repeated())
            {
                task.ApplyRepeat(true);

                UpdateTask(task);
            }
            else
                UpdateArchived(task, true);
        }

        protected void GetParents(Task task, ref List<Task> parents, int searchDepth = -1, int currentDepth = 0)
        {
            if (currentDepth == searchDepth)
                return;
            List<Task> newParents = FindTask(task.ParentIds);
            foreach (var parent in newParents)
            {
                if (parents.Contains(parent))
                    continue;
                parents.Add(parent);
                GetParents(parent, ref parents, searchDepth, currentDepth + 1);
            }
        }

        public List<Task> GetRecursiveParents(Task task, int searchDepth = -1)
        {
            if (FindTask(task.Id) == null)
                return new List<Task>();
            List<Task> parents = new List<Task>();
            GetParents(task, ref parents, searchDepth, 0);
            parents.Add(task);
            return parents;
        }

        public List<Task> GetRecursiveParents(int taskId, int searchDepth = -1)
        {
            Task? task = FindTask(taskId);
            if (task == null)
                return new List<Task>();
            else
                return GetRecursiveParents(task, searchDepth);
        }

        protected void GetChildren(Task task, ref List<Task> children, int searchDepth = -1, int currentDepth = 0)
        {
            if (currentDepth == searchDepth)
                return;
            List<Task> newChildren = FindTask(task.ChildIds);
            foreach (var child in newChildren)
            {
                if (children.Contains(child))
                    continue;
                children.Add(child);
                GetChildren(child, ref children, searchDepth, currentDepth + 1);
            }
        }

        public List<Task> GetRecursiveChildren(Task task, int searchDepth = -1)
        {
            if (FindTask(task.Id) == null)
                return new List<Task>();
            List<Task> children = new List<Task>();
            GetChildren(task, ref children, searchDepth, 0);
            children.Add(task);
            return children;
        }

        public List<Task> GetRecursiveChildren(int taskId, int searchDepth = -1)
        {
            Task? task = FindTask(taskId);
            if (task == null)
                return new List<Task>();
            else
                return GetRecursiveChildren(task, searchDepth);
        }

        public string ValidateTask(Task task)
        {
            if (task.Start() > task.Deadline())
                return "start time can't be later then deadline";

            if (task.Repeated() && (task.Start().AddYears(task.TimeParams.repeat.years).AddMonths(task.TimeParams.repeat.months)
                + task.TimeParams.repeat.custom) < task.Deadline())
                return "repeat period must be lesser than time between start time and deadline";

            if (task.ChildIds.Contains(task.Id))
                return "task can't be child of itself";

            if (task.ParentIds.Contains(task.Id))
                return "task can't be parent of itself";

            if (task.ChildIds.Any(x => task.ParentIds.Contains(x)))
                return "task can't have same task as child and parent";
            return "";
        }

        public void Backup(bool restore = false)
        {
            saveRepo.Backup(restore);
        }

        public void Undo()
        {
            saveRepo.Undo();
        }
    }
}
