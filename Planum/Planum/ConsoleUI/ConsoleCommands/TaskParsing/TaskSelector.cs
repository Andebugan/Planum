using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System.Collections.Generic;
using System.Linq;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class TaskSelector
    {
        public List<Task> Select(List<string> filters, List<Task> tasks, ref bool parseSuccessfull,
            ITaskManager _taskManager, ITagManager _tagManager)
        {
            List<Task> selectedTasks = new List<Task>();

            // selector
            string fname;
            foreach (var filter in filters)
            {
                fname = "-sr-n";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string name = filter.Substring(fname.Length);
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
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-i";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
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
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-csn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string currentStatusName = filter.Substring(fname.Length);
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
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-csi";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
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
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-tn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string tagName = filter.Substring(fname.Length);
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
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-ti";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
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
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-pn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string parentName = filter.Substring(fname.Length);
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
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-pi";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
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
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-cn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string childName = filter.Substring(fname.Length);
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
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-sr-ci";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
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
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }
                if (!parseSuccessfull)
                    return tasks;
            }

            if (selectedTasks.Count == 0 && filters.Any(x => x.Substring(0, 3) == "-sr"))
            {
                parseSuccessfull = false;
                return tasks;
            }
            else if (selectedTasks.Count == 0)
            {
                selectedTasks = new List<Task>(tasks);
            }

            int oldCount = selectedTasks.Count;
            // not selector
            foreach (var filter in filters)
            {
                fname = "-nsr-n";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string name = filter.Substring(fname.Length);
                    foreach (var task in tasks)
                    {
                        if (task.Name == name)
                        {
                            if (selectedTasks.Contains(task))
                                selectedTasks.Remove(task);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }
                fname = "-nsr-i";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.Id == id)
                        {
                            if (selectedTasks.Contains(task))
                                selectedTasks.Remove(task);
                            added = true;
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }
                fname = "-nsr-csn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string currentStatusName = filter.Substring(fname.Length);
                    foreach (var task in tasks)
                    {
                        if (task.StatusQueueIds.Count > 0)
                        {
                            int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                            if (_tagManager.FindTag(statusTagId) != null &&
                                _tagManager.FindTag(statusTagId).Name == currentStatusName)
                            {
                                if (selectedTasks.Contains(task))
                                    selectedTasks.Remove(task);
                                added = true;
                            }
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nsr-csi";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.StatusQueueIds.Count > 0)
                        {
                            int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                            if (statusTagId == id)
                            {
                                if (selectedTasks.Contains(task))
                                    selectedTasks.Remove(task);
                                added = true;
                            }
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nsr-tn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string tagName = filter.Substring(fname.Length);
                    foreach (var task in tasks)
                    {
                        if (task.TagIds.Count > 0)
                        {
                            foreach (var tag in task.TagIds)
                            {
                                if (_tagManager.FindTag(tag) != null &&
                                _tagManager.FindTag(tag).Name == tagName)
                                {
                                    if (selectedTasks.Contains(task))
                                        selectedTasks.Remove(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nsr-ti";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.TagIds.Count > 0)
                        {
                            foreach (var tag in task.TagIds)
                            {
                                if (tag == id)
                                {
                                    if (selectedTasks.Contains(task))
                                        selectedTasks.Remove(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nsr-pn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string parentName = filter.Substring(fname.Length);
                    foreach (var task in tasks)
                    {
                        if (task.ParentIds.Count > 0)
                        {
                            foreach (var parent in task.ParentIds)
                            {
                                if (_taskManager.FindTask(parent) != null &&
                                _taskManager.FindTask(parent).Name == parentName)
                                {
                                    if (selectedTasks.Contains(task))
                                        selectedTasks.Remove(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nsr-pi";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.ParentIds.Count > 0)
                        {
                            foreach (var parent in task.ParentIds)
                            {
                                if (parent == id)
                                {
                                    if (selectedTasks.Contains(task))
                                        selectedTasks.Remove(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nsr-cn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    string childName = filter.Substring(fname.Length);
                    foreach (var task in tasks)
                    {
                        if (task.ChildIds.Count > 0)
                        {
                            foreach (var child in task.ChildIds)
                            {
                                if (_taskManager.FindTask(child) != null &&
                                _taskManager.FindTask(child).Name == childName)
                                {
                                    if (selectedTasks.Contains(task))
                                        selectedTasks.Remove(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nsr-ci";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.ChildIds.Count > 0)
                        {
                            foreach (var child in task.ChildIds)
                            {
                                if (child == id)
                                {
                                    if (selectedTasks.Contains(task))
                                        selectedTasks.Remove(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                if (!parseSuccessfull)
                    return tasks;
            }

            if (selectedTasks.Count == oldCount && filters.Any(x => x.Substring(0, 4) == "-nsr"))
            {
                parseSuccessfull = false;
                return tasks;
            }

            return selectedTasks;
        }
    }
}
