using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

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

            bool showDescription = false;
            bool showTags = false;
            bool showStatus = false;
            bool showParent = false;
            bool showChildren = false;
            bool showStatusQueue = false;
            bool showStartTime = false;
            bool showDeadline = false;
            bool showRepeatPeriod = false;
            bool showArchivedTasks = false;
            bool showOnlyArchivedTasks = false;
            bool showOverdueTasks = false;
            bool showTodayTasks = false;
            bool showNotOverdueTasks = false;

            TaskCommandParser parser = new TaskCommandParser();
            parseSuccessfull = parser.Parse(ref filters, argsList,
                ref showDescription,
                ref showTags,
                ref showStatus,
                ref showParent,
                ref showChildren,
                ref showStatusQueue,
                ref showStartTime,
                ref showDeadline,
                ref showRepeatPeriod,
                ref showArchivedTasks,
                ref showOnlyArchivedTasks,
                ref showOverdueTasks,
                ref showTodayTasks,
                ref showNotOverdueTasks);

            if (showDescription || showTags || showStatus ||
                showParent || showChildren || showStatusQueue ||
                showStartTime || showDeadline || showRepeatPeriod ||
                showArchivedTasks || showOnlyArchivedTasks || showOverdueTasks ||
                showTodayTasks || showNotOverdueTasks)
                parseSuccessfull = false;

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
                        TaskSelector taskSelector = new TaskSelector();
                        List<Task> selectedTasks = taskSelector.Select(filters, tasks, ref parseSuccessfull, _taskManager, _tagManager);

                        bool hasSelectors = false;
                        foreach (string filter in filters)
                        {
                            if (filter.Substring(0, 3) == "-sr")
                                hasSelectors = true;
                        }

                        if (hasSelectors)
                            filteredTasks = selectedTasks;

                        TaskFilter taskFilter = new TaskFilter();
                        filteredTasks = taskFilter.Filter(filters, filteredTasks, ref parseSuccessfull, _taskManager, _tagManager);
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
                "   -f[option] - filter, filters tasks by some criterion(set subtraction), can be used multiple times\n" +
                "   -sr[option] - selector, selects from tasks according to a given criterion\n" +
                "       (set addition), can be used multiple times\n" +
                "   -nf[options] - \"not\" filter, removes tasks matching the filter from result\n" +
                "   -nsr[options] - \"not\" selector, removes tasks matching the selector from result\n" +
                "   filter (-f/-nf) and selector (-sr/-nsr) options:\n" +
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
