using Alba.CsConsoleFormat;
using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.UI;
using Planum.Model.Entities;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Planum.ConsoleUI.ConsoleCommands.TaskCommands
{
    public class ShowTaskCaledarCommand : BaseCommand
    {
        TaskManager taskManager;

        TaskIdsOption idsOption;
        TaskNamesOption namesOption;
        TaskIdsOption parentIdsOption;
        TaskNamesOption parentNamesOption;
        TaskIdsOption childIdsOption;
        TaskNamesOption childNamesOption;

        TaskIdsOption recursiveParentIdsOption;
        TaskNamesOption recursiveParentNamesOption;
        IntValueOption recursiveParentDepthOption = new IntValueOption("rpd", "specifies recursive parents search depth", "[depth]", 1, onlyPositive: true);

        TaskIdsOption recursiveChildIdsOption;
        TaskNamesOption recursiveChildNamesOption;
        IntValueOption recursiveChildDepthOption = new IntValueOption("rcd", "specifies recursive children search depth", "[depth]", 1, onlyPositive: true);

        BoolSettingOption showNotStartedOption = new BoolSettingOption("ns", "show not started", "", false);
        BoolSettingOption showInProgressOption = new BoolSettingOption("ip", "show in progress", "", false);
        BoolSettingOption showOverdueOption = new BoolSettingOption("od", "show overdue", "", false);
        TimespanOption lessTimeBeforeStartThanOption = new TimespanOption("tbs", "show tasks with start time within provided time range from now, value format: " + ArgumentParser.GetTimeSpanFormat(), "[timespan]", TimeSpan.MaxValue);
        TimespanOption lessTimeBeforeDeadlineThanOption = new TimespanOption("tbd", "show tasks with deadline within provided time range from now, value format: " + ArgumentParser.GetTimeSpanFormat(), "[timespan]", TimeSpan.MaxValue);

        DateTimeOption showAllFromDateOption = new DateTimeOption("ft", "show tasks existing from provided time, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.MinValue);
        DateTimeOption showAllBeforeDateOption = new DateTimeOption("bt", "show tasks existing from before provided time, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.MaxValue);
        BoolSettingOption showStartedOnly = new BoolSettingOption("so", "show started only", "", false);
        BoolSettingOption hideRepeatedDisplay = new BoolSettingOption("hr", "hide repeated tasks display", "", false);

        BoolSettingOption showArchivedOption = new BoolSettingOption("a", "show archived tasks", "", false);
        BoolSettingOption showArchivedOnlyOption = new BoolSettingOption("ao", "show only archived tasks", "", false);
        BoolSettingOption showNoParentsOption = new BoolSettingOption("np", "show tasks with no parents", "", false);

        BoolSettingOption showOnlyTimedOption = new BoolSettingOption("st", "show only timed tasks", "", false);
        BoolSettingOption showIfHasParents = new BoolSettingOption("hp", "show if has parents", "", false);
        BoolSettingOption hideNameArrowOption = new BoolSettingOption("ha", "hide name arrow option", "", false);

        BoolSettingOption showDayScale = new BoolSettingOption("d", "show day scale", "", false);
        BoolSettingOption showWeekScale = new BoolSettingOption("w", "show week scale", "", false);
        BoolSettingOption showYearScale = new BoolSettingOption("y", "show year scale", "", false);

        DateTimeOption displayStartOption = new DateTimeOption("ds", "define time from wich display starts, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.Now, defaultValNow:true);
        DateTimeOption displayEndOption = new DateTimeOption("de", "define time from wich display ends, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.Now, defaultValNow: true);

        BoolSettingOption moveNamesToTheLeft = new BoolSettingOption("mnl", "move task names to the left", "", false);
        BoolSettingOption showReferenceCalendar = new BoolSettingOption("src", "show reference calendar without any tasks", "", false);
        
        public ShowTaskCaledarCommand(TaskManager taskManager) : base("calendar", "shows tasks in calendar format, note that all applied filters stack", "[options]")
        {
            this.taskManager = taskManager;

            idsOption = new TaskIdsOption(taskManager, "i", "show with provided task ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(idsOption);

            namesOption = new TaskNamesOption(taskManager, "n", "show with provided task names", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(namesOption);

            parentIdsOption = new TaskIdsOption(taskManager, "pi", "show with provided parent ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(parentIdsOption);

            parentNamesOption = new TaskNamesOption(taskManager, "pn", "show with provided parent names", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(parentNamesOption);

            childIdsOption = new TaskIdsOption(taskManager, "ci", "show with provided children ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(childIdsOption);

            childNamesOption = new TaskNamesOption(taskManager, "cn", "show with provided children names", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(childNamesOption);

            recursiveParentIdsOption = new TaskIdsOption(taskManager, "rpi", "recursively add parents of tasks", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(recursiveParentIdsOption);

            recursiveParentNamesOption = new TaskNamesOption(taskManager, "rpn", "recursively add parents of tasks with names", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(recursiveParentNamesOption);

            recursiveChildIdsOption = new TaskIdsOption(taskManager, "rci", "recursively add children of tasks", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(recursiveChildIdsOption);

            recursiveChildNamesOption = new TaskNamesOption(taskManager, "rcn", "recursively add children of tasks with names", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(recursiveChildNamesOption);

            options.Add(recursiveParentDepthOption);
            options.Add(recursiveChildDepthOption);

            options.Add(showNotStartedOption);
            options.Add(showInProgressOption);
            options.Add(showOverdueOption);
            options.Add(lessTimeBeforeStartThanOption);
            options.Add(lessTimeBeforeDeadlineThanOption);
            options.Add(showAllFromDateOption);
            options.Add(showAllBeforeDateOption);
            options.Add(showStartedOnly);
            options.Add(hideRepeatedDisplay);
            options.Add(hideNameArrowOption);

            options.Add(showArchivedOption);
            options.Add(showArchivedOnlyOption);
            options.Add(showNoParentsOption);

            options.Add(showOnlyTimedOption);
            options.Add(showIfHasParents);

            options.Add(showDayScale);
            options.Add(showWeekScale);
            options.Add(showYearScale);

            options.Add(displayStartOption);
            options.Add(displayEndOption);

            options.Add(moveNamesToTheLeft);
            options.Add(showReferenceCalendar);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            List<Task> tasks = new List<Task>();

            if (!showReferenceCalendar.value)
            {
                tasks = taskManager.FindTask();
            }

            List<Task> result = new List<Task>(tasks);

            List<int> recursiveParents = new List<int>();
            if (recursiveParentIdsOption.Used || recursiveParentNamesOption.Used)
            {
                int depth = -1;
                if (recursiveParentDepthOption.Used)
                    depth = recursiveParentDepthOption.value;
                foreach (var taskId in recursiveParentIdsOption.value)
                {
                    recursiveParents = recursiveParents.Concat(taskManager.GetRecursiveParents(taskId, depth).Select(x => x.Id).ToList()).ToList();
                }
                recursiveParents = recursiveParents.Distinct().ToList();

                foreach (var taskId in recursiveParentNamesOption.value)
                {
                    recursiveParents = recursiveParents.Concat(taskManager.GetRecursiveParents(taskId, depth).Select(x => x.Id).ToList()).ToList();
                }
                recursiveParents = recursiveParents.Distinct().ToList();
            }

            List<int> recursiveChildren = new List<int>();
            if (recursiveChildIdsOption.Used || recursiveChildNamesOption.Used)
            {
                int depth = -1;
                if (recursiveChildDepthOption.Used)
                    depth = recursiveChildDepthOption.value;
                foreach (var taskId in recursiveChildIdsOption.value)
                {
                    recursiveChildren = recursiveChildren.Concat(taskManager.GetRecursiveChildren(taskId, depth).Select(x => x.Id).ToList()).ToList();
                }
                recursiveChildren = recursiveChildren.Distinct().ToList();

                foreach (var taskId in recursiveChildNamesOption.value)
                {
                    recursiveChildren = recursiveChildren.Concat(taskManager.GetRecursiveChildren(taskId, depth).Select(x => x.Id).ToList()).ToList();
                }
                recursiveChildren = recursiveChildren.Distinct().ToList();
            }

            foreach (var task in tasks)
            {
                if (idsOption.Used && !idsOption.value.Contains(task.Id))
                    result.Remove(task);
                if (namesOption.Used && !namesOption.value.Contains(task.Id))
                    result.Remove(task);
                if (parentIdsOption.Used)
                    if (!parentIdsOption.value.Exists(x => task.ParentIds.Contains(x)))
                        result.Remove(task);
                if (parentNamesOption.Used)
                    if (!parentNamesOption.value.Exists(x => task.ParentIds.Contains(x)))
                        result.Remove(task);
                if (childIdsOption.Used)
                    if (!childIdsOption.value.Exists(x => task.ChildIds.Contains(x)))
                        result.Remove(task);
                if (childNamesOption.Used)
                    if (!childNamesOption.value.Exists(x => task.ChildIds.Contains(x)))
                        result.Remove(task);
                if (showArchivedOnlyOption.Used)
                    if (task.Archived == false)
                        result.Remove(task);
                if (!showArchivedOption.Used && !showArchivedOnlyOption.Used)
                    if (task.Archived)
                        result.Remove(task);
                if (showNoParentsOption.Used)
                    if (task.ParentIds.Count > 0)
                        result.Remove(task);
                if (recursiveParentIdsOption.Used || recursiveParentNamesOption.Used)
                    if (!recursiveParents.Contains(task.Id))
                        result.Remove(task);
                if (recursiveChildIdsOption.Used || recursiveChildNamesOption.Used)
                    if (!recursiveChildren.Contains(task.Id))
                        result.Remove(task);
            }

            List<Task> tasks_result = new List<Task>(result);

            foreach (var task in result)
            {
                if (showNotStartedOption.Used && !(!task.HasStarted() && task.Timed()))
                    tasks_result.Remove(task);
                if (showInProgressOption.Used && !(task.HasStarted() && !task.IsOverdue() && task.Timed()))
                    tasks_result.Remove(task);
                if (showOverdueOption.Used && !(task.IsOverdue() && task.Timed()))
                    tasks_result.Remove(task);
                if (lessTimeBeforeDeadlineThanOption.Used && !(DateTime.Now < task.TimeParams.Deadline && DateTime.Now - task.TimeParams.Deadline <= lessTimeBeforeDeadlineThanOption.value && task.Timed()))
                    tasks_result.Remove(task);
                if (lessTimeBeforeStartThanOption.Used && !(DateTime.Now < task.TimeParams.Start && DateTime.Now - task.TimeParams.Start <= lessTimeBeforeStartThanOption.value && task.Timed()))
                    tasks_result.Remove(task);
                if (showAllBeforeDateOption.Used && !((task.TimeParams.Start < showAllBeforeDateOption.value || task.TimeParams.Deadline < showAllBeforeDateOption.value) && task.Timed()))
                    tasks_result.Remove(task);
                if (showAllFromDateOption.Used && !((task.TimeParams.Start > showAllFromDateOption.value || task.TimeParams.Deadline > showAllFromDateOption.value) && task.Timed()))
                    tasks_result.Remove(task);
                if (showStartedOnly.Used && !(task.HasStarted() && task.Timed()))
                    tasks_result.Remove(task);
                if (showOnlyTimedOption.Used && !(task.Timed()))
                    tasks_result.Remove(task);
                if (showIfHasParents.Used && !(task.ParentIds.Count > 0))
                    tasks_result.Remove(task);
                if (!task.Timed())
                    tasks_result.Remove(task);
            }

            if (tasks_result.Count == 0 && !showReferenceCalendar.value)
            {
                ConsoleFormat.PrintWarning("tasks matching entered parameters were not found");
                return;
            }

            if (displayStartOption.Used || displayEndOption.Used)
            {
                if (displayStartOption.value > displayEndOption.value)
                {
                    ConsoleFormat.PrintError("display start time can't be later than end time");
                    return;
                }
            }

            // automatic timesort for result
            // overdue/in progress/before start time/timeless
            IComparer<Task> taskComparer = new TaskTimeComparer();
            tasks_result.Sort(taskComparer);
            tasks_result.Reverse();
            tasks_result = tasks_result.Distinct().ToList();

            if (showDayScale.Used)
                RenderDays(tasks_result);
            else if (showWeekScale.Used)
                RenderWeeks(tasks_result);
            else if (showYearScale.Used)
                RenderYears(tasks_result);
            else
                RenderMonths(tasks_result);
        }

        public int CompareDate(DateTime a, DateTime b, string mode = "")
        {
            if (mode == "h")
            {
                a = new DateTime(a.Year, a.Month, a.Day, a.Hour, 0, 0);
                b = new DateTime(b.Year, b.Month, b.Day, b.Hour, 0, 0);
            }

            if (mode == "d")
            {
                a = new DateTime(a.Year, a.Month, a.Day, 0, 0, 0);
                b = new DateTime(b.Year, b.Month, b.Day, 0, 0, 0);
            }    

            if (mode == "w")
            {
                a = new DateTime(a.Year, a.Month, a.Day, 0, 0, 0);
                b = new DateTime(b.Year, b.Month, b.Day, 0, 0, 0);

                DateTime start = a;
                DateTime end = a;

                while (start.DayOfWeek != DayOfWeek.Monday || end.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (start.DayOfWeek != DayOfWeek.Monday)
                        start = start.AddDays(-1);
                    if (end.DayOfWeek != DayOfWeek.Sunday)
                        end = end.AddDays(1);

                    if (start == b)
                        return 0;
                    if (end == b)
                        return 0;
                }

                if (b < start)
                    return 1;
                if (b > end)
                    return -1;
            }

            if (mode == "m")
            {
                a = new DateTime(a.Year, a.Month, 1, 0, 0, 0);
                b = new DateTime(b.Year, b.Month, 1, 0, 0, 0);
            }

            if (mode == "y")
            {
                a = new DateTime(a.Year, 1, 1, 0, 0, 0);
                b = new DateTime(b.Year, 1, 1, 0, 0, 0);
            }

            if (a < b)
                return -1;
            if (a == b)
                return 0;
            if (a > b)
                return 1;

            return 0;
        }

        // ========= - red
        // ~~~~~~~~~ - yellow
        // --------- - dark yellow
        // # - deadline - shows relative deadline time
        // * - today - shows relative today time
        // ~~-- - shows time as well
        // @ - start time
        // today - ~-
        // |today|        |start time|--------|deadline|
        // |start time|~~~~~~~~|today|--------|deadline|
        // |start time|        |deadline|=====|today|

        string[] months =
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        string[] days =
        {
            "Monday",
            "Tuesday",
            "Wensday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"

        };

        string? FormTaskString(DateTime date, ref Task task, ref List<Task> repeatedTasks, int singleStrLen, int taskNameLen, ref ConsoleColor strColor, string scale, ref string taskName, bool last = false)
        {
            float cellStep = 0.0f;
            string compareScale = "";
            if (scale == "d" || scale == "m")
                compareScale = "d";
            if (scale == "y")
                compareScale = "m";
            if (scale == "w")
                compareScale = "w";
            if (scale == "d" || scale == "m" || scale == "w")
                cellStep = 24.0f / singleStrLen;
            if (scale == "y")
                cellStep = (float)DateTime.DaysInMonth(date.Year, date.Month) / singleStrLen;

            if (CompareDate(date, task.Start(), compareScale) < 0 && !repeatedTasks.Contains(task))
                    return null;
            if (CompareDate(task.Deadline(), date, scale) < 0 && !task.IsOverdue())
                return null;
            if (task.Archived)
                return null;

            string result = "";

            DateTime cursor = date;
            if (scale == "d" || scale == "w" || scale == "m")
                cursor = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            if (scale == "y")
                cursor = new DateTime(date.Year, date.Month, 1, 0, 0, 0);

            DateTime startCursor = cursor;
            DateTime endCursor = cursor;

            if (scale == "d" || scale == "w" || scale == "m")
                endCursor = endCursor.AddDays(1);
            if (scale == "y")
                endCursor = endCursor.AddMonths(1);

            strColor = ConsoleColor.DarkYellow;
            if (task.InProgress())
                strColor = ConsoleColor.Yellow;
            if (task.IsOverdue())
                strColor = ConsoleColor.Red;
            if (moveNamesToTheLeft.Used)
            {
                taskName =  "[" + task.Id + "] " + task.Name;
                if (taskName.Length > taskNameLen)
                {
                    taskName = taskName.Substring(0, taskNameLen - 3);
                    taskName += "...";
                }
            }

            Task lastDrawnTask = task;

            while (result.Length < singleStrLen && cursor < endCursor)
            {
                DateTime cursorIncremented = cursor;
                DateTime cursorDecremented = cursor;
                if (scale == "d" || scale == "m" || scale == "w")
                {
                    cursorIncremented = cursor.AddHours(cellStep);
                    cursorDecremented = cursor.AddHours(-cellStep);
                }
                if (scale == "y")
                {
                    cursorIncremented = cursor.AddDays(cellStep);
                    cursorDecremented = cursor.AddDays(-cellStep);
                }

                if (cursorIncremented > endCursor)
                    cursorIncremented = endCursor;
                if (cursorDecremented < startCursor)
                    cursorDecremented = startCursor;

                if (cursor <= task.Start() && cursorIncremented > task.Start())
                    result += "@";
                else if (cursor < task.Start())
                    result += " ";
                else if (cursor > task.Deadline() && task.IsOverdue())
                {
                    if (cursorIncremented > DateTime.Now && cursorDecremented < DateTime.Now)
                    {
                        result += "[" + task.Id + "]";
                        if (!moveNamesToTheLeft.Used)
                            result += " " + task.Name;
                        break;
                    }
                    else if (cursor > task.Deadline() && cursor < DateTime.Now)
                        result += "=";

                }
                else if (cursorIncremented > task.Deadline() && cursorDecremented < task.Deadline())
                {
                    if (task.IsOverdue())
                        result += "#";
                    else
                    {
                        lastDrawnTask = new Task(task);
                        result += "[" + task.Id + "]";
                        if (!moveNamesToTheLeft.Used)
                            result += " " + task.Name;
                        if (task.Repeated() && !hideRepeatedDisplay.Used)
                        {
                            taskManager.ApplyRepeatPeriod(ref task);
                            if (!repeatedTasks.Contains(task))
                                repeatedTasks.Add(task);
                        }
                        else
                            break;
                    }

                }
                else if (cursor > task.Start() && cursor < DateTime.Now && cursor < task.Deadline())
                    result += "~";
                else if (cursor > task.Start() && cursor > DateTime.Now && cursor < task.Deadline())
                    result += "-";

                cursor = cursorIncremented;
            }

            if (last && !hideNameArrowOption.Used)
            {
                if (!task.IsOverdue())
                {
                    if (CompareDate(date, lastDrawnTask.Deadline(), compareScale) < 0)
                    {
                        result += "[" + lastDrawnTask.Id + "]";
                        if (!moveNamesToTheLeft.Used)
                            result += " " + lastDrawnTask.Name;
                        result += ">";
                    }
                }
                else
                {
                    if (CompareDate(date, DateTime.Now, compareScale) < 0)
                    {
                        result += "[" + lastDrawnTask.Id + "]";
                        if (!moveNamesToTheLeft.Used)
                            result += " " + lastDrawnTask.Name;
                        result += ">";
                    }
                }
            }

            if (result.Length > singleStrLen)
            {
                string name = "";
                if (!lastDrawnTask.IsOverdue())
                {
                    if (CompareDate(date, lastDrawnTask.Deadline(), compareScale) == 0)
                    {
                        name = "[" + lastDrawnTask.Id + "]";
                        if (!moveNamesToTheLeft.Used)
                            name += " " + lastDrawnTask.Name;

                        if (name.Length > singleStrLen)
                        {
                            name = name.Substring(0, singleStrLen - 3);
                            name += "...";
                        }
                    }
                    if (CompareDate(date, lastDrawnTask.Deadline(), compareScale) < 0 && last && !hideNameArrowOption.Used)
                    {
                        name = "[" + lastDrawnTask.Id + "]";
                        if (!moveNamesToTheLeft.Used)
                            name += " " + lastDrawnTask.Name;
                        name += ">";
                        if (name.Length > singleStrLen)
                        {
                            name = name.Substring(0, singleStrLen - 4);
                            name += "...>";
                        }
                    }
                }
                else
                {
                    if (CompareDate(date, DateTime.Now, compareScale) == 0)
                    {
                        name = "[" + lastDrawnTask.Id + "]";
                        if (!moveNamesToTheLeft.Used)
                            name += " " + lastDrawnTask.Name;
                        if (name.Length > singleStrLen)
                        {
                            name = name.Substring(0, singleStrLen - 3);
                            name += "...";
                        }

                    }
                    if (CompareDate(date, DateTime.Now, compareScale) < 0 && lastDrawnTask.IsOverdue() && last && !hideNameArrowOption.Used)
                    {
                        name = "[" + lastDrawnTask.Id + "]";
                        if (!moveNamesToTheLeft.Used)
                            name += " " + lastDrawnTask.Name;
                        name += ">";
                        if (name.Length > singleStrLen)
                        {
                            name = name.Substring(0, singleStrLen - 4);
                            name += "...>";
                        }
                    }
                }

                result = result.Substring(0, singleStrLen - name.Length);
                result += name;
            }

            return result;
        }

        string GetHourString(ref int singleStrLen)
        {
            // form hour string
            string[] hourDisplay = { "0 ", "3 ", "6 ", "9 ", "12", "15", "18", "21" };

            int spaceCount = singleStrLen;
            foreach (var val in hourDisplay)
                spaceCount -= val.Length;

            if (spaceCount <= hourDisplay.Length)
            {
                hourDisplay = new string[] { "0 ", "6 ", "12", "18" };
                spaceCount = singleStrLen;
                foreach (var val in hourDisplay)
                    spaceCount -= val.Length;
            }

            if (spaceCount <= hourDisplay.Length)
            {
                hourDisplay = new string[] { "12" };
                spaceCount = singleStrLen;
                foreach (var val in hourDisplay)
                    spaceCount -= val.Length;
            }

            string hourString = "";
            int spaceVal = spaceCount / hourDisplay.Length;
            string spaces = "";
            for (int i = 0; i < spaceVal; i++)
                spaces += " ";

            foreach (var val in hourDisplay)
                hourString += val + spaces;
            singleStrLen = hourString.Length;
            return hourString;
        }

        public void RenderDays(List<Task> tasks)
        {
            Document doc = new Document();

            int singleStrLen = Console.BufferWidth - 8;
            int nameLen = 0;

            if (moveNamesToTheLeft.Used)
            {
                nameLen = singleStrLen / 6;
                singleStrLen = singleStrLen - nameLen;
            }

            // display setup
            string hourString = GetHourString(ref singleStrLen);

            DateTime start = displayStartOption.value;
            DateTime end = displayEndOption.value;
            DateTime currentDate = start;

            foreach (var task in tasks)
            {
                if (task.Repeated() && task.Timed())
                {
                    while (CompareDate(task.Deadline(), start, "d") < 0 && !task.IsOverdue())
                        task.ApplyRepeat();
                }
            }

            List<Task> repeatedTasks = new List<Task>();
            while (currentDate <= end)
            {
                ConsoleColor textColor = ConsoleColor.Gray;
                ConsoleColor headerColor = ConsoleColor.White;
                if (CompareDate(DateTime.Now, start, "d") == 0)
                {
                    textColor = ConsoleColor.White;
                    headerColor = ConsoleColor.Cyan;
                }

                Grid grid = new Grid();

                int columnSpan = 1;
                if (moveNamesToTheLeft.Used)
                    columnSpan = 2;

                grid.Children.Add(new Cell(currentDate.ToString(ArgumentParser.DateFormat) + " [" + currentDate.DayOfWeek.ToString() + "]") { Align = Align.Center, Color = headerColor, ColumnSpan = columnSpan });
                if (moveNamesToTheLeft.Used)
                    grid.Children.Add(new Cell("") { });
                grid.Children.Add(new Cell(hourString) { Align = Align.Center, Color = textColor});

                Stack stack = new Stack();
                Stack nameStack = new Stack();
                List<Task> temp = new List<Task>();
                foreach (var task in tasks)
                {
                    Task taskTemp = task;
                    ConsoleColor strColor = textColor;
                    string taskName = "";
                    string? res = FormTaskString(currentDate, ref taskTemp, ref repeatedTasks, singleStrLen, nameLen, ref strColor, "d", ref taskName, true);
                    if (res != null)
                    {
                        stack.Children.Add(new Cell(res) { Color = strColor });
                        nameStack.Children.Add(new Cell(taskName) { Color = strColor});
                    }
                    if (res != null)
                        temp.Add(taskTemp);
                }
                foreach (var task in tasks)
                    if (!temp.Contains(task))
                        temp.Add(task);
                tasks = new List<Task>(temp);

                currentDate = currentDate.AddDays(1);
                if (moveNamesToTheLeft.Used)
                    grid.Children.Add(new Cell(nameStack));
                grid.Children.Add(stack);
                if (moveNamesToTheLeft.Used)
                    grid.Columns.Add(nameLen);
                grid.Columns.Add(singleStrLen);
                doc.Children.Add(grid);
            }

            ConsoleRenderer.RenderDocument(doc);
        }

        public void RenderWeeks(List<Task> tasks)
        {
            Document doc = new Document();

            int singleStrLen = Console.BufferWidth - 10;
            int nameLen = 0;
            int dayLength = 0;

            dayLength = singleStrLen / 7;
            if (moveNamesToTheLeft.Used)
            {
                dayLength = singleStrLen / 8;
                nameLen = dayLength;
            }

            DateTime start = displayStartOption.value;
            DateTime end = displayEndOption.value;
            DateTime currentDate = start;

            Grid grid = new Grid();

            if (moveNamesToTheLeft.Used)
                grid.Columns.Add(dayLength);
            for (int i = 0; i < 7; i++)
                grid.Columns.Add(dayLength);

            ConsoleColor textColor = ConsoleColor.Gray;
            ConsoleColor headerColor = ConsoleColor.White;

            if (CompareDate(DateTime.Now, currentDate, "m") == 0)
            {
                textColor = ConsoleColor.White;
                headerColor = ConsoleColor.Cyan;
            }

            int columnSpan = 7;
            if (moveNamesToTheLeft.Used)
                columnSpan = 8;

            grid.Children.Add(new Cell(months[currentDate.Month - 1] + " [" + currentDate.Month.ToString() + "." + currentDate.Year.ToString() + "]") { Align = Align.Center, Color = headerColor, ColumnSpan = columnSpan });

            if (moveNamesToTheLeft.Used)
                grid.Children.Add(new Cell());

            foreach (var dayOfWeek in days)
            {
                grid.Children.Add(new Cell(dayOfWeek) { Align = Align.Center });
            }

            while (currentDate.DayOfWeek != DayOfWeek.Monday)
            {
                currentDate = currentDate.AddDays(-1);
            }

            foreach (var task in tasks)
            {
                if (task.Repeated() && task.Timed())
                {
                    while (CompareDate(task.Deadline(), start, "w") < 0)
                        task.ApplyRepeat();
                }
            }

            while (CompareDate(currentDate, end, "w") <= 0)
            {
                DateTime next = currentDate.AddDays(7);

                bool first = true;
                List<Task> repeatedTasks = new List<Task>();
                while (currentDate.Day != next.Day)
                {
                    Stack stack = new Stack();

                    textColor = ConsoleColor.Gray;
                    headerColor = ConsoleColor.White;

                    int dayNum = currentDate.Day;
                    string dayStr = dayNum.ToString();
                    if (dayStr.Length == 2)
                        dayStr = " " + dayStr + " ";
                    else
                        dayStr = " 0" + dayStr + " ";


                    if (CompareDate(DateTime.Now, currentDate, "m") == 0)
                    {
                        textColor = ConsoleColor.White;
                        headerColor = ConsoleColor.Cyan;
                    }

                    ConsoleColor backColor = textColor;
                    if (CompareDate(currentDate, DateTime.Now, "d") == 0)
                        backColor = ConsoleColor.Cyan;
                    else if (currentDate.Month == DateTime.Now.Month)
                        backColor = ConsoleColor.White;
                    else if (currentDate.Month != next.AddMonths(-1).Month)
                        backColor = ConsoleColor.DarkGray;

                    stack.Children.Add(new Cell(dayStr) { Align = Align.Left, Background = backColor, Color = ConsoleColor.Black });

                    Stack nameStack = new Stack();
                    nameStack.Children.Add(new Cell(""));

                    List<Task> temp = new List<Task>();
                    foreach (var task in tasks)
                    {
                        Task taskTemp = task;
                        ConsoleColor strColor = textColor;
                        bool isLast = false;
                        if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                            isLast = true;
                        string taskName = "";
                        string? res = FormTaskString(currentDate, ref taskTemp, ref repeatedTasks, dayLength, nameLen, ref strColor, "w", ref taskName, isLast);
                        if (res != null)
                        {
                            stack.Children.Add(new Cell(res) { Color = strColor });
                            if (first)
                                nameStack.Children.Add(new Cell(taskName) { Color = strColor});
                        }
                        if (res != null)
                            temp.Add(taskTemp);
                    }

                    foreach (var task in tasks)
                        if (!temp.Contains(task))
                            temp.Add(task);
                    tasks = new List<Task>(temp);

                    if (first && moveNamesToTheLeft.Used)
                    {
                        grid.Children.Add(new Cell(nameStack));
                        first = false;
                    }

                    grid.Children.Add(new Cell(stack));

                    currentDate = currentDate.AddDays(1);
                }
            }

            doc.Children.Add(grid);
            ConsoleRenderer.RenderDocument(doc);
        }

        public void RenderMonths(List<Task> tasks)
        {
            Document doc = new Document();

            int singleStrLen = Console.BufferWidth - 10;
            int nameLen = 0;
            int dayLength = singleStrLen / 7;

            if (moveNamesToTheLeft.Used)
            {
                dayLength = singleStrLen / 8;
                nameLen = dayLength;
            }

            DateTime start = displayStartOption.value;
            DateTime end = displayEndOption.value;
            DateTime currentDate = start;

            foreach (var task in tasks)
            {
                if (task.Repeated() && task.Timed())
                {
                    while (CompareDate(task.Deadline(), start, "m") < 0)
                        task.ApplyRepeat();
                }
            }

            while (CompareDate(currentDate, end, "m") <= 0)
            {
                ConsoleColor textColor = ConsoleColor.Gray;
                ConsoleColor headerColor = ConsoleColor.White;
                if (CompareDate(DateTime.Now, currentDate, "m") == 0)
                {
                    textColor = ConsoleColor.White;
                    headerColor = ConsoleColor.Cyan;
                }

                Grid grid = new Grid();

                if (moveNamesToTheLeft.Used)
                    grid.Columns.Add(dayLength);
                for (int i = 0; i < 7; i++)
                    grid.Columns.Add(dayLength);

                int columnSpan = 7;
                if (moveNamesToTheLeft.Used)
                    columnSpan = 8;

                grid.Children.Add(new Cell(months[currentDate.Month - 1] + " [" + currentDate.Month.ToString() + "." + currentDate.Year.ToString() + "]") { Align = Align.Center, Color = headerColor, ColumnSpan = columnSpan });

                if (moveNamesToTheLeft.Used)
                    grid.Children.Add(new Cell());

                foreach (var dayOfWeek in days)
                {
                    grid.Children.Add(new Cell(dayOfWeek) { Align = Align.Center });
                }

                DateTime next = currentDate.AddMonths(1);

                while (currentDate.Day != 1)
                {
                    currentDate = currentDate - new TimeSpan(24, 0, 0);
                }

                while (currentDate.DayOfWeek != DayOfWeek.Monday)
                {
                    currentDate = currentDate - new TimeSpan(24, 0, 0);
                }

                List<Task> repeatedTasks = new List<Task>();
                int addedDaysCount = 0;

                while (currentDate.Month != next.Month)
                {
                    addedDaysCount += 1;
                    Stack stack = new Stack();

                    int dayNum = currentDate.Day;
                    string dayStr = dayNum.ToString();
                    if (dayStr.Length == 2)
                        dayStr = " " + dayStr + " ";
                    else
                        dayStr = " 0" + dayStr + " ";

                    ConsoleColor backColor = textColor;
                    if (CompareDate(currentDate, DateTime.Now, "d") == 0)
                        backColor = ConsoleColor.Cyan;
                    if (currentDate.Month != next.AddMonths(-1).Month)
                        backColor = ConsoleColor.DarkGray;

                    stack.Children.Add(new Cell(dayStr) { Align = Align.Left, Background = backColor, Color = ConsoleColor.Black });

                    Stack nameStack = new Stack();
                    nameStack.Children.Add(new Cell(""));

                    List<Task> temp = new List<Task>();
                    bool isLast = false;
                    foreach (var task in tasks)
                    {
                        Task taskTemp = task;
                        ConsoleColor strColor = textColor;
                        if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                            isLast = true;
                        if (currentDate.Day == DateTime.DaysInMonth(currentDate.Year, currentDate.Month) && currentDate.Month != next.AddMonths(-2).Month)
                            isLast = true;
                        string taskName = "";
                        string? res = FormTaskString(currentDate, ref taskTemp, ref repeatedTasks, dayLength, nameLen, ref strColor, "m", ref taskName, isLast);
                        if (res != null)
                        {
                            stack.Children.Add(new Cell(res) { Color = strColor });
                            if (currentDate.DayOfWeek == DayOfWeek.Sunday || isLast)
                                nameStack.Children.Add(new Cell(taskName) { Color = strColor });
                        }
                        if (res != null)
                            temp.Add(taskTemp);
                    }

                    foreach (var task in tasks)
                        if (!temp.Contains(task))
                            temp.Add(task);
                    tasks = new List<Task>(temp);

                    if ((currentDate.DayOfWeek == DayOfWeek.Sunday || isLast) && moveNamesToTheLeft.Used)
                    {
                        grid.Children.Insert(grid.Children.Count - addedDaysCount + 1, new Cell(nameStack));
                        addedDaysCount = 0;
                    }

                    grid.Children.Add(new Cell(stack));

                    currentDate = currentDate.AddDays(1);
                }

                doc.Children.Add(grid);
            }
            ConsoleRenderer.RenderDocument(doc);
        }

        public void RenderYears(List<Task> tasks)
        {
            Document doc = new Document();

            int singleStrLen = Console.BufferWidth - 15;

            int nameLen = 0;
            int monthLength = singleStrLen / 12;

            if (moveNamesToTheLeft.Used)
            {
                monthLength = singleStrLen / 13;
                nameLen = monthLength;
            }

            DateTime start = displayStartOption.value;
            DateTime end = displayEndOption.value;
            DateTime currentDate = start;

            foreach (var task in tasks)
            {
                if (task.Repeated() && task.Timed())
                {
                    while (CompareDate(task.Deadline(), start, "y") < 0)
                        task.ApplyRepeat();
                }
            }

            while (CompareDate(currentDate, end, "y") <= 0)
            {
                ConsoleColor textColor = ConsoleColor.Gray;
                ConsoleColor headerColor = ConsoleColor.White;
                if (CompareDate(DateTime.Now, currentDate, "y") == 0)
                {
                    textColor = ConsoleColor.White;
                    headerColor = ConsoleColor.Cyan;
                }

                Grid grid = new Grid();

                if (moveNamesToTheLeft.Used)
                    grid.Columns.Add(monthLength);
                for (int i = 0; i < 12; i++)
                    grid.Columns.Add(monthLength);

                int columnSpan = 12;
                if (moveNamesToTheLeft.Used)
                    columnSpan = 13;
                grid.Children.Add(new Cell(currentDate.Year.ToString()) { Align = Align.Center, Color = headerColor, ColumnSpan = columnSpan });

                if (moveNamesToTheLeft.Used)
                    grid.Children.Add(new Cell(""));
                for (int i = 0; i < 12; i++)
                {
                    if (currentDate.Year == DateTime.Now.Year && i + 1 == DateTime.Now.Month)
                        grid.Children.Add(new Cell(months[i] + " [" + (i + 1).ToString() + "]") { Align = Align.Center, Color = ConsoleColor.Cyan });
                    else
                        grid.Children.Add(new Cell(months[i] + " [" + (i + 1).ToString() + "]") { Align = Align.Center, Color = textColor });
                }

                int next = currentDate.Year + 1;

                while (currentDate.Month != 1)
                {
                    currentDate = currentDate.AddMonths(-1);
                }

                List<Task> repeatedTasks = new List<Task>();
                int addedMonthsCount = 0;
                Stack nameStack = new Stack();
                List<string> names = new List<string>();

                while (currentDate.Year != next)
                {
                    addedMonthsCount += 1;
                    Stack stack = new Stack();

                    List<Task> temp = new List<Task>();
                    bool isLast = false;
                    foreach (var task in tasks)
                    {
                        Task taskTemp = task;
                        ConsoleColor strColor = textColor;
                        isLast = false;
                        if (currentDate.Month == 12)
                            isLast = true;
                        string taskName = "";
                        string? res = FormTaskString(currentDate, ref taskTemp, ref repeatedTasks, monthLength, nameLen, ref strColor, "y", ref taskName, isLast);
                        if (res != null)
                        {
                            stack.Children.Add(new Cell(res) { Color = strColor });
                            if (!names.Contains(taskName))
                            {
                                names.Add(taskName);
                                nameStack.Children.Add(new Cell(taskName) { Color = strColor });
                            }
                        }
                        if (res != null)
                            temp.Add(taskTemp);
                    }
                    foreach (var task in tasks)
                        if (!temp.Contains(task))
                            temp.Add(task);
                    tasks = new List<Task>(temp);

                    if ((currentDate.Month == 12 || isLast) && moveNamesToTheLeft.Used)
                    {
                        grid.Children.Insert(grid.Children.Count - addedMonthsCount + 1, new Cell(nameStack));
                        addedMonthsCount = 0;
                    }

                    grid.Children.Add(new Cell(stack));

                    currentDate = currentDate.AddMonths(1);
                }

                doc.Children.Add(grid);
            }
            ConsoleRenderer.RenderDocument(doc);
        }
    }
}
