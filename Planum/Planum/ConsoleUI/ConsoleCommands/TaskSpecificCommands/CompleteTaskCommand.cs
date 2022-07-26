using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Planum.ConsoleUI.ConsoleCommands
{
    internal class CompleteTaskCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;
        ITagManager _tagManager;

        public CompleteTaskCommand(ITaskManager taskManager, ITagManager tagManager, IUserManager userManager)
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
            Serilog.Log.Information("complete task command was called");

            List<string> filters = new List<string>();
            List<string> argsList = new List<string>(args);
            bool parseSuccessfull = true;

            argsList.Remove("complete");
            argsList.Remove("task");

            Dictionary<string, bool> boolParams = new Dictionary<string, bool>()
            {

            };

            Dictionary<string, string> stringParams = new Dictionary<string, string>()
            {

            };

            TaskCommandParser parser = new TaskCommandParser();
            parseSuccessfull = parser.Parse(ref filters, argsList, ref boolParams, ref stringParams, "complete");

            if (parseSuccessfull)
            {
                if (filters.Count == 0)
                {
                    List<Task> tasks = _taskManager.GetAllTasks();
                    foreach (var task in tasks)
                    {
                        _taskManager.CompleteTask(task.Id);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("completed all tasks successfully\n");
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

                    if (filteredTasks.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("there are no tasks with the described parameters in the system\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }

                    foreach (var task in filteredTasks)
                    {
                        _taskManager.CompleteTask(task.Id);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("completed selected tasks successfully\n");
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
            return "completes task, all by default\n" +
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
            return "complete task\n" +
                "complete [-f[option]] [-sr[option]] task";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "complete")
                return true;
            return false;
        }
    }
}
