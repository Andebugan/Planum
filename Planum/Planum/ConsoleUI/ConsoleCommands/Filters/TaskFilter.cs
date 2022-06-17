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
            foreach (var filter in filters)
            {
                if (filter.Length > 4 && filter.Substring(0, 4) == "-f-n")
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string name = filter.Substring(4);
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
                        parseSuccessfull = false;
                }
                else if (filter.Length > 4 && filter.Substring(0, 4) == "-f-i")
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(4));
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
                        parseSuccessfull = false;
                }
                else if (filter.Length > 6 && filter.Substring(0, 6) == "-f-csn")
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string currentStatusName = filter.Substring(6);
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
                        parseSuccessfull = false;
                }
                else if (filter.Length > 6 && filter.Substring(0, 6) == "-f-csi")
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(6));
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
                        parseSuccessfull = false;
                }
                else if (filter.Length > 5 && filter.Substring(0, 5) == "-f-tn")
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string tagName = filter.Substring(5);
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
                        parseSuccessfull = false;
                }
                else if (filter.Length > 5 && filter.Substring(0, 5) == "-f-ti")
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(5));
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
                        parseSuccessfull = false;
                }
                else if (filter.Length > 5 && filter.Substring(0, 5) == "-f-pn")
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string parentName = filter.Substring(5);
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
                        parseSuccessfull = false;
                }
                else if (filter.Length > 5 && filter.Substring(0, 5) == "-f-pi")
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(5));
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
                        parseSuccessfull = false;
                }
                else if (filter.Length > 5 && filter.Substring(0, 5) == "-f-cn")
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    string childName = filter.Substring(5);
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
                        parseSuccessfull = false;
                }
                else if (filter.Length > 5 && filter.Substring(0, 5) == "-f-ci")
                {
                    bool added = false;
                    List<Task> tempList = new List<Task>();
                    int id = int.Parse(filter.Substring(5));
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

            return filteredTasks;
        }
    }
}
