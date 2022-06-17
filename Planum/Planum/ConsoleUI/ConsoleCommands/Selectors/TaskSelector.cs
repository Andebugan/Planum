using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System.Collections.Generic;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class TaskSelector
    {
        public List<Task> Select(List<string> filters, List<Task> tasks, ref bool parseSuccessfull,
            ITaskManager _taskManager, ITagManager _tagManager)
        {
            List<Task> selectedTasks = new List<Task>();

            // selector
            foreach (var filter in filters)
            {
                if (filter.Length > 5 && filter.Substring(0, 5) == "-sr-n")
                {
                    bool added = false;
                    string name = filter.Substring(5);
                    foreach (var task in tasks)
                    {
                        if (task.Name == name)
                        {
                            if (!selectedTasks.Contains(task))
                                selectedTasks.Add(task);
                            added = true;
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length > 5 && filter.Substring(0, 5) == "-sr-i")
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(5));
                    foreach (var task in tasks)
                    {
                        if (task.Id == id)
                        {
                            if (!selectedTasks.Contains(task))
                                selectedTasks.Add(task);
                            added = true;
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length > 7 && filter.Substring(0, 7) == "-sr-csn")
                {
                    bool added = false;
                    string currentStatusName = filter.Substring(7);
                    foreach (var task in tasks)
                    {
                        if (task.StatusQueueIds.Count > 0)
                        {
                            int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                            if (_tagManager.FindTag(statusTagId) != null &&
                                _tagManager.FindTag(statusTagId).Name == currentStatusName)
                            {
                                if (!selectedTasks.Contains(task))
                                    selectedTasks.Add(task);
                                added = true;
                            }
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length > 7 && filter.Substring(0, 7) == "-sr-csi")
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(7));
                    foreach (var task in tasks)
                    {
                        if (task.StatusQueueIds.Count > 0)
                        {
                            int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                            if (statusTagId == id)
                            {
                                if (!selectedTasks.Contains(task))
                                    selectedTasks.Add(task);
                                added = true;
                            }
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length > 6 && filter.Substring(0, 6) == "-sr-tn")
                {
                    bool added = false;
                    string tagName = filter.Substring(6);
                    foreach (var task in tasks)
                    {
                        if (task.TagIds.Count > 0)
                        {
                            foreach (var tag in task.TagIds)
                            {
                                if (_tagManager.FindTag(tag) != null &&
                                _tagManager.FindTag(tag).Name == tagName)
                                {
                                    if (!selectedTasks.Contains(task))
                                        selectedTasks.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length > 6 && filter.Substring(0, 6) == "-sr-ti")
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(6));
                    foreach (var task in tasks)
                    {
                        if (task.TagIds.Count > 0)
                        {
                            foreach (var tag in task.TagIds)
                            {
                                if (tag == id)
                                {
                                    if (!selectedTasks.Contains(task))
                                        selectedTasks.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length > 6 && filter.Substring(0, 6) == "-sr-pn")
                {
                    bool added = false;
                    string parentName = filter.Substring(6);
                    foreach (var task in tasks)
                    {
                        if (task.ParentIds.Count > 0)
                        {
                            foreach (var parent in task.ParentIds)
                            {
                                if (_taskManager.FindTask(parent) != null &&
                                _taskManager.FindTask(parent).Name == parentName)
                                {
                                    if (!selectedTasks.Contains(task))
                                        selectedTasks.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length > 6 && filter.Substring(0, 6) == "-sr-pi")
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(6));
                    foreach (var task in tasks)
                    {
                        if (task.ParentIds.Count > 0)
                        {
                            foreach (var parent in task.ParentIds)
                            {
                                if (parent == id)
                                {
                                    if (!selectedTasks.Contains(task))
                                        selectedTasks.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length > 6 && filter.Substring(0, 6) == "-sr-cn")
                {
                    bool added = false;
                    string childName = filter.Substring(6);
                    foreach (var task in tasks)
                    {
                        if (task.ChildIds.Count > 0)
                        {
                            foreach (var child in task.ChildIds)
                            {
                                if (_taskManager.FindTask(child) != null &&
                                _taskManager.FindTask(child).Name == childName)
                                {
                                    if (!selectedTasks.Contains(task))
                                        selectedTasks.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length > 6 && filter.Substring(0, 6) == "-sr-ci")
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(6));
                    foreach (var task in tasks)
                    {
                        if (task.ChildIds.Count > 0)
                        {
                            foreach (var child in task.ChildIds)
                            {
                                if (child == id)
                                {
                                    if (!selectedTasks.Contains(task))
                                        selectedTasks.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                        parseSuccessfull = false;
                }
                else if (filter.Length < 3 || (filter.Substring(0, 2) != "-f" && filter.Substring(0, 3) != "-sr"))
                {
                    parseSuccessfull = false;
                    break;
                }

                if (!parseSuccessfull)
                    break;
            }

            return selectedTasks;
        }
    }
}
