using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.UI;
using Planum.Model.Entities;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.TaskCommands
{
    public class ShowTreeCommand: BaseCommand
    {
        TaskIdsOption idsOption;
        TaskNamesOption namesOption;
        TaskIdsOption parentIdsOption;
        TaskNamesOption parentNamesOption;
        TaskIdsOption childIdsOption;
        TaskNamesOption childNamesOption;

        TaskIdsOption startTaskIdsOption;
        TaskNamesOption startNamesOption;

        BoolSettingOption showNotStartedOption = new BoolSettingOption("ns", "show not started", "", false);
        BoolSettingOption showInProgressOption = new BoolSettingOption("ip", "show in progress", "", false);
        BoolSettingOption showOverdueOption = new BoolSettingOption("od", "show overdue", "", false);
        TimespanOption lessTimeBeforeStartThanOption = new TimespanOption("tbs", "show tasks with start time within provided time range from now, value format: " + ArgumentParser.GetTimeSpanFormat(), "[timespan]", TimeSpan.MaxValue);
        TimespanOption lessTimeBeforeDeadlineThanOption = new TimespanOption("tbd", "show tasks with deadline within provided time range from now, value format: " + ArgumentParser.GetTimeSpanFormat(), "[timespan]", TimeSpan.MaxValue);

        DateTimeOption showAllFromDateOption = new DateTimeOption("ft", "show tasks existing from provided time, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.MinValue);
        DateTimeOption showAllBeforeDateOption = new DateTimeOption("bt", "show tasks existing from before provided time, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.MaxValue);

        BoolSettingOption showArchivedOption = new BoolSettingOption("a", "show archived tasks", "", false);
        BoolSettingOption showArchivedOnlyOption = new BoolSettingOption("ao", "show only archived tasks", "", false);
        BoolSettingOption showNoParentsOption = new BoolSettingOption("np", "show tasks with no parents", "", false);

        BoolSettingOption showOnlyTimedOption = new BoolSettingOption("ot", "show only timed option", "", false);
        BoolSettingOption timedStatusOption = new BoolSettingOption("dts", "disable time status", "", false);
        BoolSettingOption showTimeOption = new BoolSettingOption("t", "show time if time status used", "", false);
        BoolSettingOption showIfHasParents = new BoolSettingOption("hp", "show if has parents", "", false);

        IntValueOption treeDepthOption = new IntValueOption("td", "define tree depth", "[depth]", 0);

        TaskManager taskManager;

        public ShowTreeCommand(TaskManager taskManager) : base("tree", "shows tasks in tree format, note that all applied filters stack", "[options]")
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

            startTaskIdsOption = new TaskIdsOption(taskManager, "st", "start tasks ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(startTaskIdsOption);

            startNamesOption = new TaskNamesOption(taskManager, "stn", "start task names", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(startNamesOption);

            options.Add(showNotStartedOption);
            options.Add(showInProgressOption);
            options.Add(showOverdueOption);
            options.Add(lessTimeBeforeStartThanOption);
            options.Add(lessTimeBeforeDeadlineThanOption);
            options.Add(showAllFromDateOption);
            options.Add(showAllBeforeDateOption);

            options.Add(showArchivedOption);
            options.Add(showArchivedOnlyOption);
            options.Add(showNoParentsOption);

            options.Add(showOnlyTimedOption);
            options.Add(showIfHasParents);

            options.Add(treeDepthOption);
            options.Add(timedStatusOption);
            options.Add(showTimeOption);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            List<Task> tasks = taskManager.FindTask();
            List<Task> result = new List<Task>(tasks);

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
                if (showOnlyTimedOption.Used && !(task.Timed()))
                    tasks_result.Remove(task);
                if (showIfHasParents.Used && !(task.ParentIds.Count > 0))
                    tasks_result.Remove(task);
            }

            if (tasks_result.Count == 0)
            {
                ConsoleFormat.PrintWarning("tasks matching entered parameters were not found");
                return;
            }

            // automatic timesort for result
            // overdue/in progress/before start time/timeless
            IComparer<Task> taskComparer = new TaskTimeComparer();
            tasks_result.Sort(taskComparer);
            tasks_result.Reverse();
            tasks_result = tasks_result.Distinct().ToList();

            if (treeDepthOption.Used)
            {
                if (treeDepthOption.value < 0)
                {
                    ConsoleFormat.PrintWarning("incorrect tree depth value");
                    return;
                }
            }

            RenderTasks(tasks_result, taskManager);
        }

        public void PrintTask(int level, Task task, ref List<int> displayed, List<int> filteredTasks)
        {
            if (!filteredTasks.Contains(task.Id))
            {
                if (!displayed.Contains(task.Id))
                    displayed.Add(task.Id);
                List<Task> temp = taskManager.FindTask(task.ChildIds);
                foreach (var child in temp)
                    PrintTask(level, child, ref displayed, filteredTasks);
                return;
            }

            string indent = "";
            for (int i = 0; i < level; i++)
                indent += "    ";
            displayed.Add(task.Id);

            Console.Write(indent); 

            if (task.Timed() && !timedStatusOption.value)
            {
                if (task.IsOverdue())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[" + task.Id.ToString() + "] ");
                    if (showTimeOption.value)
                        Console.Write((DateTime.Now - task.TimeParams.Deadline).ToString(ArgumentParser.TimeSpanFormat) + " ");
                    Console.WriteLine(task.Name);
                }
                else if (task.InProgress())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[" + task.Id.ToString() + "] ");
                    if (showTimeOption.value)
                        Console.Write((DateTime.Now - task.TimeParams.Deadline).ToString(ArgumentParser.TimeSpanFormat) + " ");
                    Console.WriteLine(task.Name);
                }
                else if (!task.HasStarted())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[" + task.Id.ToString() + "] ");
                    if (showTimeOption.value)
                        Console.Write((DateTime.Now - task.TimeParams.Start).ToString(ArgumentParser.TimeSpanFormat) + " ");
                    Console.WriteLine(task.Name);
                }
            }
            else
            {
                Console.WriteLine("[" + task.Id.ToString() + "] " + task.Name);
            }

            Console.ForegroundColor = ConsoleColor.White;

            if (treeDepthOption.Used && level == treeDepthOption.value)
                return;

            List<Task> children = taskManager.FindTask(task.ChildIds);
            level++;
            foreach (var child in children)
                PrintTask(level, child, ref displayed, filteredTasks);
        }

        public void RenderTasks(List<Task> tasks, TaskManager taskManager)
        {
            List<Task> firstLevelTasks = new List<Task>();

            if (!startTaskIdsOption.Used && !startNamesOption.Used)
                foreach (var task in tasks)
                {
                    if (task.ParentIds.Count == 0)
                        firstLevelTasks.Add(task);
                }           
            else
            {
                if (startTaskIdsOption.Used)
                {
                    foreach (var task in tasks)
                    {
                        if (startTaskIdsOption.value.Contains(task.Id))
                            firstLevelTasks.Add(task);
                    }
                }

                if (startNamesOption.Used)
                {
                    foreach (var task in tasks)
                    {
                        if (startNamesOption.value.Contains(task.Id))
                            firstLevelTasks.Add(task);
                    }
                }
            }

            firstLevelTasks = firstLevelTasks.Distinct().ToList();

            List<int> filteredIds = tasks.Select(x => x.Id).ToList();

            bool lastFirstLevel = true;

            foreach (var task in firstLevelTasks)
            {
                lastFirstLevel = true;
                List<int> displayed = new List<int>();
                PrintTask(0, task, ref displayed, filteredIds);
                if (displayed.Count > 1)
                {
                    Console.WriteLine();
                    lastFirstLevel = false;
                }
            }
            if (lastFirstLevel)
                Console.WriteLine();
        }
    }
}
