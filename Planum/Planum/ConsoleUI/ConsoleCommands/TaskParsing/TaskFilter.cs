using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System.Collections.Generic;
namespace Planum.ConsoleUI.ConsoleCommands
{
    public class TaskFilter
    {
        public List<Task> Filter(List<string> filters, List<Task> tasks, ref bool parseSuccessfull,
            ITaskManager _taskManager, ITagManager _tagManager)
        {
            List<Task> filteredTasks = tasks;
            // filter
            string fname;
            foreach (var filter in filters)
            {
                fname = "-f-n";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string name = filter.Substring(fname.Length);
                    foreach (var task in filteredTasks)
                    {
                        if (task.Name == name)
                        {
                            if (!tempList.Contains(task))
                                tempList.Add(task);
                            added = true;
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-f-i";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.Id == id)
                        {
                            if (!tempList.Contains(task))
                                tempList.Add(task);
                            added = true;
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-f-n";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string currentStatusName = filter.Substring(fname.Length);
                    foreach (var task in tasks)
                    {
                        if (task.StatusQueueIds.Count > 0)
                        {
                            int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                            if (_tagManager.FindTag(statusTagId) != null &&
                                _tagManager.FindTag(statusTagId).Name == currentStatusName)
                            {
                                if (!tempList.Contains(task))
                                    tempList.Add(task);
                                added = true;
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-f-csi";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.StatusQueueIds.Count > 0)
                        {
                            int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                            if (statusTagId == id)
                            {
                                if (!tempList.Contains(task))
                                    tempList.Add(task);
                                added = true;
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-f-tn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
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
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-f-ti";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.TagIds.Count > 0)
                        {
                            foreach (var tag in task.TagIds)
                            {
                                if (tag == id)
                                {
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-f-pn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
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
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-f-pi";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.ParentIds.Count > 0)
                        {
                            foreach (var parent in task.ParentIds)
                            {
                                if (parent == id)
                                {
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-f-cn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
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
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-f-ci";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.ChildIds.Count > 0)
                        {
                            foreach (var child in task.ChildIds)
                            {
                                if (child == id)
                                {
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
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

            // not filter
            foreach (var filter in filters)
            {
                fname = "-nf-n";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string name = filter.Substring(fname.Length);
                    foreach (var task in filteredTasks)
                    {
                        if (task.Name != name)
                        {
                            if (!tempList.Contains(task))
                                tempList.Add(task);
                            added = true;
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nf-i";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.Id == id)
                        {
                            if (!tempList.Contains(task))
                                tempList.Add(task);
                            added = true;
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nf-csn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string currentStatusName = filter.Substring(fname.Length);
                    foreach (var task in tasks)
                    {
                        if (task.StatusQueueIds.Count > 0)
                        {
                            int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                            if (_tagManager.FindTag(statusTagId) != null &&
                                _tagManager.FindTag(statusTagId).Name != currentStatusName)
                            {
                                if (!tempList.Contains(task))
                                    tempList.Add(task);
                                added = true;
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nf-csi";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.StatusQueueIds.Count > 0)
                        {
                            int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                            if (statusTagId != id)
                            {
                                if (!tempList.Contains(task))
                                    tempList.Add(task);
                                added = true;
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nf-tn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string tagName = filter.Substring(fname.Length);
                    foreach (var task in tasks)
                    {
                        if (task.TagIds.Count > 0)
                        {
                            foreach (var tag in task.TagIds)
                            {
                                if (_tagManager.FindTag(tag) != null &&
                                    _tagManager.FindTag(tag).Name != tagName)
                                {
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nf-ti";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.TagIds.Count > 0)
                        {
                            foreach (var tag in task.TagIds)
                            {
                                if (tag != id)
                                {
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nf-pn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string parentName = filter.Substring(fname.Length);
                    foreach (var task in tasks)
                    {
                        if (task.ParentIds.Count > 0)
                        {
                            foreach (var parent in task.ParentIds)
                            {
                                if (_taskManager.FindTask(parent) != null &&
                                    _taskManager.FindTask(parent).Name != parentName)
                                {
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nf-pi";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.ParentIds.Count > 0)
                        {
                            foreach (var parent in task.ParentIds)
                            {
                                if (parent != id)
                                {
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nf-cn";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string childName = filter.Substring(fname.Length);
                    foreach (var task in tasks)
                    {
                        if (task.ChildIds.Count > 0)
                        {
                            foreach (var child in task.ChildIds)
                            {
                                if (_taskManager.FindTask(child) != null &&
                                    _taskManager.FindTask(child).Name != childName)
                                {
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
                    if (!added)
                    {
                        parseSuccessfull = false;
                        break;
                    }
                    continue;
                }

                fname = "-nf-ci";
                if (filter.Length > fname.Length && filter.Substring(0, fname.Length) == fname)
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(fname.Length));
                    foreach (var task in tasks)
                    {
                        if (task.ChildIds.Count > 0)
                        {
                            foreach (var child in task.ChildIds)
                            {
                                if (child != id)
                                {
                                    if (!tempList.Contains(task))
                                        tempList.Add(task);
                                    added = true;
                                }
                            }
                        }
                    }
                    filteredTasks = tempList;
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
            return filteredTasks;
        }
    }
}
