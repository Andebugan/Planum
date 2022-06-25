using Planum.ConsoleUI.ConsoleViews;
using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowCommand: ICommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public ShowCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        public void ShowCurrentUser()
        {
            Serilog.Log.Information("show current user command was called");
            if (_userManager.CurrentUser == null)
                return;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("id: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_userManager.CurrentUser.Id);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("login: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_userManager.CurrentUser.Login);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("password: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_userManager.CurrentUser.Password);
            Console.WriteLine();
        }

        public void ShowAllUsers()
        {
            Serilog.Log.Information("show all users command was called");
            List<User> users = _userManager.GetAllUsers();
            if (users.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("there are no users in the system\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("existing users:\n");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (User user in users)
            {
                Console.WriteLine(user.Login);
            }
            Console.WriteLine();
        }

        public void ShowAllTags(bool showCategory = false, bool showDescription = false, List<string>? filters = null)
        {
            Serilog.Log.Information("show all tags command was called");

            bool parseSuccessfull = true;

            List<Tag> tags = _tagManager.GetAllTags();
            List<Tag> filteredTags = tags;

            if (filters != null && filters.Count != 0)
            {

                if (tags.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("there are no tags in the system\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                TagSelector tagSelector = new TagSelector();
                List<Tag> selectedTags = tagSelector.Select(filters, tags, ref parseSuccessfull, _taskManager, _tagManager);

                bool hasSelectors = false;
                foreach (string filter in filters)
                {
                    if (filter.Substring(0, 3) == "-sr")
                        hasSelectors = true;
                }

                if (hasSelectors)
                    filteredTags = selectedTags;

                TagFilter tagFilter = new TagFilter();
                filteredTags = tagFilter.Filter(filters, filteredTags, ref parseSuccessfull, _taskManager, _tagManager);
            }

            if (!parseSuccessfull)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("incorrect filter parameters\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            TagListView tagListView = new TagListView();
            tagListView.RenderTags(filteredTags, showCategory, showDescription);
        }

        public void ShowAllTasks(
            bool showDescription = false,
            bool showTags = false,
            bool showStatus = false,
            bool showParent = false,
            bool showChildren = false,
            bool showStatusQueue = false,
            bool showStartTime = false,
            bool showDeadline = false,
            bool showRepeatPeriod = false,
            bool showArchivedTasks = false,
            bool showOnlyArchivedTasks = false,
            bool showOverdueTasks = false,
            bool showTodayTasks = false,
            bool showNotOverdueTasks = false,
            List<string>? filters = null)
        {
            Serilog.Log.Information("show all tasks command was called");

            List<Task> tasks = new List<Task>();
            if (showOnlyArchivedTasks)
                tasks = _taskManager.GetAllTasks(true);
            else if (showArchivedTasks)
                tasks = _taskManager.GetAllTasks(null);
            else
                tasks = _taskManager.GetAllTasks(false);

            if (tasks.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                if (showOnlyArchivedTasks)
                    Console.WriteLine("there are no archived tasks in the system\n");
                else if (showArchivedTasks)
                    Console.WriteLine("there are no tasks in the system\n");
                else
                    Console.WriteLine("there are no unarchived tasks in the system\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            bool parseSuccessfull = true;
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

            TaskListView taskListView = new TaskListView();
            taskListView.RenderTasks(filteredTasks, _tagManager, _taskManager,
                showDescription,
                showTags,
                showStatus,
                showParent,
                showChildren,
                showStatusQueue,
                showStartTime,
                showDeadline,
                showRepeatPeriod);
        }

        public void Execute(string command)
        {
            string[] args = command.Split();

            if (args[args.Length - 1] == "user")
            {
                if (args.Length == 2 && _userManager.CurrentUser == null)
                {
                    ShowAllUsers();
                    return;
                }

                if (args.Length == 2 && _userManager.CurrentUser != null)
                {
                    ShowCurrentUser();
                    return;
                }
            }

            if (args[args.Length - 1] == "tag")
            {
                bool parseSuccessfull = true;
                bool showDescription = false;
                bool showCategory = false;

                List<string> filters = new List<string>();

                List<string> argsList = new List<string>(args);
                argsList.Remove("show");
                argsList.Remove("tag");
                TagCommandParser parser = new TagCommandParser();
                parseSuccessfull = parser.Parse(ref filters, argsList, ref showDescription, ref showCategory);

                if (parseSuccessfull)
                {
                    ShowAllTags(showCategory, showDescription, filters);
                    return;
                }
            }

            if (args[args.Length - 1] == "task")
            {
                bool parseSuccessfull = true;
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
                string displayType = "l";

                List<string> filters = new List<string>();

                List<string> argsList = new List<string>(args);
                argsList.Remove("show");
                argsList.Remove("task");

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

                if (parseSuccessfull)
                {
                    ShowAllTasks(
                        showDescription,
                        showTags,
                        showStatus,
                        showParent,
                        showChildren,
                        showStatusQueue,
                        showStartTime,
                        showDeadline,
                        showRepeatPeriod,
                        showArchivedTasks,
                        showOnlyArchivedTasks,
                        showOverdueTasks,
                        showTodayTasks,
                        showNotOverdueTasks,
                        filters);
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Incorrect command parameters\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            if (_userManager.CurrentUser == null)
                return "displays objects, shows all existing by default";
            else
                return "displays objects, shows all existing by default\n" +
                    "flags:\n" +
                    "   tag:\n" +
                    "   -c - show category\n" +
                    "   -d - show description\n" +
                    "   -f[options] - filter, filters tags by some criterion (set subtraction)\n" +
                    "   -sr[options] - selector, selects from tags according to a given criterion (set addition)\n" +
                    "   -nf[options] - \"not\" filter, removes tags matching the filter from result\n" +
                    "   -nsr[options] - \"not\" selector, removes tags matching the selector from result\n" +
                    "   filter (-f/-nf) and selector (-sr/-nsr) options:\n" +
                    "       -c={value} - filter by category\n" +
                    "       -n={value} - filter by name\n" +
                    "       -i={value} - filter by id\n" +
                    "   task:\n" +
                    "       -all - show full info about task\n" +
                    "       -d - show description\n" +
                    "       -t - show tags\n" +
                    "       -s - show status\n" +
                    "       -p - show parents\n" +
                    "       -c - show children\n" +
                    "       -sq - show status queue\n" +
                    "       -st - show start time\n" +
                    "       -dl - show deadline\n" +
                    "       -r - show repeat period\n" +
                    "       -a - show archived tasks\n" +
                    "       -ao - show only archived tasks\n" +
                    "       -od - show overdue tasks\n" +
                    "       -nod - show not overdue tasks\n" +
                    "       -tt - show today tasks\n" +
                    "       -f[option] - filter, filters tasks by some criterion(set subtraction), can be used multiple times\n" +
                    "       -sr[option] - selector, selects from tasks according to a given criterion\n" +
                    "           (set addition), can be used multiple times\n" +
                    "       -nf[options] - \"not\" filter, removes tasks matching the filter from result\n" +
                    "       -nsr[options] - \"not\" selector, removes tasks matching the selector from result\n" +
                    "       filter (-f/-nf) and selector (-sr/-nsr) options:\n" +
                    "           -i={value} - id\n" +
                    "           -n={value} - name\n" +
                    "           -csi={value} - current status id\n" +
                    "           -csn={value} - current status name\n" +
                    "           -ti={value} - tag id\n" +
                    "           -tn={value} - tag name\n" +
                    "           -pi={value} - parent id\n" +
                    "           -pn={value} - parent name\n" +
                    "           -ci={value} - child id\n" +
                    "           -cn={value} - child name";
        }

        public string GetName()
        {
            if (_userManager.CurrentUser == null)
                return "show user";
            else
                return "show [-l] [-d] [-a] task\n" +
                    "show [-c] [-d] [-f[options]] [-sr[options]] tag\n" +
                    "show user";
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "show")
                return true;
            return false;
        }

        public bool IsAvaliable()
        {
            return true;
        }
    }
}
