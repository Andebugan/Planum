using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ArchiveTaskCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;
        ITagManager _tagManager;

        public ArchiveTaskCommand(ITaskManager taskManager, ITagManager tagManager, IUserManager userManager)
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
            Serilog.Log.Information("archive task command was called");

            List<string> filters = new List<string>();
            List<string> argsList = new List<string>(args);
            bool parseSuccessfull = true;

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
                    List<Task> tasks = _taskManager.GetAllTasks();
                    foreach (var task in tasks)
                    {
                        _taskManager.ArchiveTask(task.Id);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("archived all tasks successfully\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                else
                {
                    List<Task> tasks = new List<Task>();
                    tasks = _taskManager.GetAllTasks(false);

                    if (tasks.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("there are no unarchived tasks in the system\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }

                    parseSuccessfull = true;
                    List<Task> filteredTasks = tasks;

                    if (filters != null && filters.Count != 0)
                    {

                        if (tasks.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("there are no tasks in the system\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }

                        List<Task> selectedTasks = new List<Task>();

                        // selector
                        foreach (var filter in filters)
                        {
                            if (filter.Substring(0, 5) == "-sr-n")
                            {
                                string name = filter.Substring(5);
                                foreach (var task in tasks)
                                {
                                    if (task.Name == name)
                                        selectedTasks.Add(task);
                                }
                            }
                            else if (filter.Substring(0, 5) == "-sr-i")
                            {
                                int id = int.Parse(filter.Substring(5));
                                foreach (var task in tasks)
                                {
                                    if (task.Id == id)
                                        selectedTasks.Add(task);
                                }
                            }
                            else if (filter.Substring(0, 7) == "-sr-csn")
                            {
                                string currentStatusName = filter.Substring(7);
                                foreach (var task in tasks)
                                {
                                    if (task.StatusQueueIds.Count > 0)
                                    {
                                        int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                                        if (_tagManager.FindTag(statusTagId) != null &&
                                            _tagManager.FindTag(statusTagId).Name == currentStatusName)
                                        {
                                            selectedTasks.Add(task);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (filter.Substring(0, 7) == "-sr-csi")
                            {
                                int id = int.Parse(filter.Substring(7));
                                foreach (var task in tasks)
                                {
                                    if (task.StatusQueueIds.Count > 0)
                                    {
                                        int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                                        if (statusTagId == id)
                                        {
                                            selectedTasks.Add(task);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (filter.Substring(0, 6) == "-sr-tn")
                            {
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
                                                selectedTasks.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (filter.Substring(0, 6) == "-sr-ti")
                            {
                                int id = int.Parse(filter.Substring(6));
                                foreach (var task in tasks)
                                {
                                    if (task.TagIds.Count > 0)
                                    {
                                        foreach (var tag in task.TagIds)
                                        {
                                            if (tag == id)
                                            {
                                                selectedTasks.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (filter.Substring(0, 6) == "-sr-pn")
                            {
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
                                                selectedTasks.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (filter.Substring(0, 6) == "-sr-pi")
                            {
                                int id = int.Parse(filter.Substring(6));
                                foreach (var task in tasks)
                                {
                                    if (task.ParentIds.Count > 0)
                                    {
                                        foreach (var parent in task.ParentIds)
                                        {
                                            if (parent == id)
                                            {
                                                selectedTasks.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (filter.Substring(0, 6) == "-sr-cn")
                            {
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
                                                selectedTasks.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (filter.Substring(0, 6) == "-sr-ci")
                            {
                                int id = int.Parse(filter.Substring(6));
                                foreach (var task in tasks)
                                {
                                    if (task.ChildIds.Count > 0)
                                    {
                                        foreach (var child in task.ChildIds)
                                        {
                                            if (child == id)
                                            {
                                                selectedTasks.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (filter.Substring(0, 2) != "-f" && filter.Substring(0, 3) != "-sr")
                            {
                                parseSuccessfull = false;
                                break;
                            }
                        }

                        if (selectedTasks.Count > 0)
                            filteredTasks = selectedTasks;

                        // filter
                        foreach (var filter in filters)
                        {
                            if (filter.Substring(0, 5) == "-f-n")
                            {
                                List<Task> tempList = new List<Task>();
                                string name = filter.Substring(5);
                                foreach (var task in filteredTasks)
                                {
                                    if (task.Name == name)
                                        tempList.Add(task);
                                }
                                filteredTasks = tempList;
                            }
                            else if (filter.Substring(0, 5) == "-f-i")
                            {
                                List<Task> tempList = new List<Task>();
                                int id = int.Parse(filter.Substring(5));
                                foreach (var task in tasks)
                                {
                                    if (task.Id == id)
                                        tempList.Add(task);
                                }
                                filteredTasks = tempList;
                            }
                            else if (filter.Substring(0, 7) == "-f-csn")
                            {
                                List<Task> tempList = new List<Task>();
                                string currentStatusName = filter.Substring(7);
                                foreach (var task in tasks)
                                {
                                    if (task.StatusQueueIds.Count > 0)
                                    {
                                        int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                                        if (_tagManager.FindTag(statusTagId) != null &&
                                            _tagManager.FindTag(statusTagId).Name == currentStatusName)
                                        {
                                            tempList.Add(task);
                                            break;
                                        }
                                    }
                                }
                                filteredTasks = tempList;
                            }
                            else if (filter.Substring(0, 7) == "-f-csi")
                            {
                                List<Task> tempList = new List<Task>();
                                int id = int.Parse(filter.Substring(7));
                                foreach (var task in tasks)
                                {
                                    if (task.StatusQueueIds.Count > 0)
                                    {
                                        int statusTagId = task.StatusQueueIds[task.CurrentStatusIndex];
                                        if (statusTagId == id)
                                        {
                                            tempList.Add(task);
                                            break;
                                        }
                                    }
                                }
                                filteredTasks = tempList;
                            }
                            else if (filter.Substring(0, 6) == "-f-tn")
                            {
                                List<Task> tempList = new List<Task>();
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
                                                tempList.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                                filteredTasks = tempList;
                            }
                            else if (filter.Substring(0, 6) == "-f-ti")
                            {
                                List<Task> tempList = new List<Task>();
                                int id = int.Parse(filter.Substring(6));
                                foreach (var task in tasks)
                                {
                                    if (task.TagIds.Count > 0)
                                    {
                                        foreach (var tag in task.TagIds)
                                        {
                                            if (tag == id)
                                            {
                                                tempList.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                                filteredTasks = tempList;
                            }
                            else if (filter.Substring(0, 6) == "-f-pn")
                            {
                                List<Task> tempList = new List<Task>();
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
                                                tempList.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                                filteredTasks = tempList;
                            }
                            else if (filter.Substring(0, 6) == "-f-pi")
                            {
                                List<Task> tempList = new List<Task>();
                                int id = int.Parse(filter.Substring(6));
                                foreach (var task in tasks)
                                {
                                    if (task.ParentIds.Count > 0)
                                    {
                                        foreach (var parent in task.ParentIds)
                                        {
                                            if (parent == id)
                                            {
                                                tempList.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                                filteredTasks = tempList;
                            }
                            else if (filter.Substring(0, 6) == "-f-cn")
                            {
                                List<Task> tempList = new List<Task>();
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
                                                tempList.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                                filteredTasks = tempList;
                            }
                            else if (filter.Substring(0, 6) == "-f-ci")
                            {
                                List<Task> tempList = new List<Task>();
                                int id = int.Parse(filter.Substring(6));
                                foreach (var task in tasks)
                                {
                                    if (task.ChildIds.Count > 0)
                                    {
                                        foreach (var child in task.ChildIds)
                                        {
                                            if (child == id)
                                            {
                                                tempList.Add(task);
                                                break;
                                            }
                                        }
                                    }
                                }
                                filteredTasks = tempList;
                            }
                            else if (filter.Substring(0, 2) != "-f" && filter.Substring(0, 3) != "-sr")
                            {
                                parseSuccessfull = false;
                                break;
                            }
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
                        _taskManager.ArchiveTask(task.Id);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("archived selected tasks successfully\n");
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
            return "archive task\n" +
                "archive [-f[option]] [-sr[option]] task";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }

        public bool IsCommand(string command)
        {
            if (command == "archive")
                return true;
            return false;
        }
    }
}
