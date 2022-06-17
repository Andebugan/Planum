using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class UnarchiveTaskCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;
        ITagManager _tagManager;

        public UnarchiveTaskCommand(ITaskManager taskManager, ITagManager tagManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
            _tagManager = tagManager;
        }

        public int TryGetIdFilter(string filterName, string parsedString, ref List<string> filters)
        {
            string filter = filterName + "=";
            if (parsedString.Length > filter.Length && parsedString.Substring(0, filter.Length) == filter)
            {
                int id;
                if (!int.TryParse(parsedString.Substring(filter.Length), out id) || id < 0)
                {
                    return -1;
                }
                filters.Add(filterName + id.ToString());
                return 0;
            }
            return 1;
        }

        public int TryGetStringFilter(string filterName, ref List<string> argsList, ref List<string> filters, ref int i)
        {
            string filter = filterName + "=";
            if (argsList[i].Length > filter.Length && argsList[i].Substring(0, filter.Length) == filter)
            {
                string filterValue = argsList[i].Substring(filter.Length);

                if (filterValue[0] == '{' && filterValue[filterValue.Length - 1] == '}')
                {
                    filterValue = filterValue.Replace("{", "");
                    filterValue = filterValue.Replace("}", "");
                }
                else if (filterValue[0] == '{' && filterValue[filterValue.Length - 1] != '}')
                {
                    i += 1;
                    while (i < argsList.Count)
                    {
                        filterValue += " " + argsList[i];
                        if (argsList[i][argsList[i].Length - 1] == '}')
                        {
                            i += 1;
                            break;
                        }
                        i += 1;
                    }

                    filterValue = filterValue.Replace("{", "");
                    filterValue = filterValue.Replace("}", "");

                    if (i == argsList.Count)
                        return -2;
                    else
                        i -= 1;
                }

                filters.Add(filterName + filterValue);
                return 0;
            }
            return 1;
        }

        public void Execute(string command)
        {
            string[] args = command.Split(' ');
            Serilog.Log.Information("unarchive task command was called");

            List<string> filters = new List<string>();
            List<string> argsList = new List<string>(args);
            bool parseSuccessfull = true;

            argsList.Remove("unarchive");
            argsList.Remove("task");

            for (int i = 0; i < argsList.Count; i++)
            {
                if (argsList[i].Substring(0, 2) == "-f" || argsList[i].Substring(0, 3) == "-sr")
                {
                    string[] intFilters =
                    {
                            "-f-i",
                            "-f-csi",
                            "-f-ti",
                            "-f-pi",
                            "-f-ci",
                            "-sr-i",
                            "-sr-csi",
                            "-sr-ti",
                            "-sr-pi",
                            "-sr-ci"
                        };

                    string[] stringFilters =
                    {
                            "-f-n",
                            "-f-csn",
                            "-f-tn",
                            "-f-pn",
                            "-f-cn",
                            "-sr-n",
                            "-sr-csn",
                            "-sr-tn",
                            "-sr-pn",
                            "-sr-cn"
                        };

                    bool needBreak = false;
                    foreach (string filter in intFilters)
                    {
                        int rc = TryGetIdFilter(filter, argsList[i], ref filters);
                        if (rc == -1)
                        {
                            needBreak = true;
                            parseSuccessfull = false;
                            break;
                        }
                    }

                    if (needBreak)
                        break;

                    foreach (string filter in stringFilters)
                    {
                        int rc = TryGetStringFilter(filter, ref argsList, ref filters, ref i);
                        if (rc == -1)
                        {
                            needBreak = true;
                            parseSuccessfull = false;
                            break;
                        }
                        else if (rc == -2)
                        {
                            needBreak = true;
                            break;
                        }
                    }

                    if (needBreak)
                        break;
                }
                else
                {
                    parseSuccessfull = false;
                    break;
                }
            }

            if (parseSuccessfull)
            {
                if (filters.Count == 0)
                {
                    List<Task> tasks = _taskManager.GetAllTasks(true);
                    foreach (var task in tasks)
                    {
                        _taskManager.UnarchiveTask(task.Id);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("unarchived all tasks successfully\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                else
                {
                    List<Task> tasks = new List<Task>();
                    tasks = _taskManager.GetAllTasks(true);

                    if (tasks.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("there are no archived tasks in the system\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }

                    parseSuccessfull = true;
                    List<Task> filteredTasks = tasks;

                    if (filters != null && filters.Count != 0)
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

                        if (selectedTasks.Count > 0)
                            filteredTasks = selectedTasks;

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
                    }

                    if (!parseSuccessfull)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect filter parameters\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }

                    foreach (var task in filteredTasks)
                    {
                        _taskManager.UnarchiveTask(task.Id);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("unarchived selected tasks successfully\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("incorrect command parameters\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            return "archives task, all by default\n" +
                "flags:\n" +
                "-f[option] - filter, filters tasks by some criterion(set subtraction), can be used multiple times\n" +
                "-sr[option] - selector, selects from tasks according to a given criterion\n" +
                "(set addition), can be used multiple times\n" +
                "   filter (-f) and selector (-sr) options:\n" +
                "       -i={value} - id\n" +
                "       -n={value} - name\n" +
                "       -csi={value} - current status id\n" +
                "       -csn={value} - current status name\n" +
                "       -ti={value} - tag id\n" +
                "       -tn={value} - tag name\n" +
                "       -pi={value} - parent id\n" +
                "       -pn={value} - parent name\n" +
                "       -ci={value} - child id\n" +
                "       -cn={value} - child name";
        }

        public string GetName()
        {
            return "unarchive task\n" +
                "unarchive [-f[option]] [-sr[option]] task";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "unarchive")
                return true;
            return false;
        }
    }
}
