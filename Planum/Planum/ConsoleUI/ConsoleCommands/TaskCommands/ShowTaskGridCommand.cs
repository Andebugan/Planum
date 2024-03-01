using Alba.CsConsoleFormat;
using Planum.ConsoleUI.CommandProcessor;
using static System.ConsoleColor;
using Planum.ConsoleUI.UI;
using Planum.Model.Entities;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.TaskCommands
{
    public class ShowTaskGridCommand : BaseCommand
    {
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
        TimespanOption lessTimeBeforeStartThanOption = new TimespanOption("tbs", "show tasks with start time within provided time range from now, value format: " + ArgumentParser.GetTimeSpanFormat(), " [timespan]", TimeSpan.MaxValue);
        TimespanOption lessTimeBeforeDeadlineThanOption = new TimespanOption("tbd", "show tasks with deadline within provided time range from now, value format: " + ArgumentParser.GetTimeSpanFormat(), " [timespan]", TimeSpan.MaxValue);

        DateTimeOption showAllFromDateOption = new DateTimeOption("ft", "show tasks existing from provided time, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.MinValue);
        DateTimeOption showAllBeforeDateOption = new DateTimeOption("bt", "show tasks existing from before provided time, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.MaxValue);

        BoolSettingOption showArchivedOption = new BoolSettingOption("a", "show archived tasks", "", false);
        BoolSettingOption showArchivedOnlyOption = new BoolSettingOption("ao", "show only archived tasks", "", false);
        BoolSettingOption showNoParentsOption = new BoolSettingOption("np", "show tasks with no parents", "", false);

        BoolSettingOption showAllOption = new BoolSettingOption("all", "show all task parameters", "", false);
        BoolSettingOption showStatusOption = new BoolSettingOption("s", "show status", "", false);
        BoolSettingOption showDescriptionOption = new BoolSettingOption("d", "show desription", "", false);
        BoolSettingOption showChildrenOption = new BoolSettingOption("c", "show children", "", false);
        BoolSettingOption showParentsOption = new BoolSettingOption("p", "show parents", "", false);
        BoolSettingOption showTimeLimits = new BoolSettingOption("tl", "show time limits", "", false);
        BoolSettingOption showRepeatParameters = new BoolSettingOption("r", "show repeat parameters", "", false);
        BoolSettingOption showChecklist = new BoolSettingOption("cl", "show checklist", "", false);
        BoolSettingOption standartDisplayOption = new BoolSettingOption("sd", "show description, time parameters, checklist", "", false);
        BoolSettingOption sortChecklistByDone = new BoolSettingOption("scd", "sort checklist by done", "", false);
        BoolSettingOption showStartedOnly = new BoolSettingOption("so", "show started only", "", false);

        IntValueOption gridColumnsOption = new IntValueOption("gc", "specify number of dispayed columns", "[column count]", 1);

        BoolSettingOption showOnlyTimedOption = new BoolSettingOption("ot", "show only timed tasks", "", false);
        BoolSettingOption showIfHasParents = new BoolSettingOption("hp", "show if has parents", "", false);

        IntValueOption showNFirstTasks = new IntValueOption("nft", "show n first tasks", "[task count]", 0);

        TaskManager taskManager;

        public ShowTaskGridCommand(TaskManager taskManager) : base("show", "shows tasks in grid format, note that all applied filters stack", "[options]")
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

            options.Add(showArchivedOption);
            options.Add(showArchivedOnlyOption);
            options.Add(showNoParentsOption);

            options.Add(showAllOption);
            options.Add(showStatusOption);
            options.Add(showDescriptionOption);
            options.Add(showChildrenOption);
            options.Add(showParentsOption);
            options.Add(showTimeLimits);
            options.Add(showRepeatParameters);
            options.Add(showChecklist);

            options.Add(standartDisplayOption);
            options.Add(gridColumnsOption);

            options.Add(showOnlyTimedOption);
            options.Add(showIfHasParents);

            options.Add(showNFirstTasks);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            List<Task> tasks = taskManager.FindTask();
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
                if (showNotStartedOption.Used && !(!task.HasStarted() || task.Timed()))
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
                if (showStartedOnly.Used && !(task.HasStarted() && task.Timed()))
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

            if (showNFirstTasks.Used)
            {
                if (showNFirstTasks.value <= 0)
                {
                    ConsoleFormat.PrintWarning("incorrect task count value");
                    return;
                }
                tasks_result = tasks_result.GetRange(0, showNFirstTasks.value);
            }

            RenderTasks(tasks_result, taskManager);
        }

        public void RenderTasks(List<Task> tasks, TaskManager taskManager)
        {
            var doc = new Document();

            int cellWidth = 35;
            int columnCount = Console.BufferWidth / (cellWidth + 4);

            if (columnCount > tasks.Count)
                columnCount = tasks.Count;

            if (gridColumnsOption.Used)
            {
                if (gridColumnsOption.value <= 0)
                {
                    ConsoleFormat.PrintWarning("tasks matching entered parameters were not found");
                    return;
                }
                columnCount = gridColumnsOption.value;
            }

            if (standartDisplayOption.value)
            {
                showDescriptionOption.value = true;
                showTimeLimits.value = true;
                showChecklist.value = true;
            }

            Grid mainGrid = new Grid();
            mainGrid.Color = DarkGray;

            foreach (var task in tasks)
            {
                Grid grid = new Grid();
                grid.Color = DarkGray;

                // show time
                if (task.Timed())
                {
                    if (task.IsOverdue())
                    {
                        grid.Children.Add(new Cell((DateTime.Now - task.TimeParams.Deadline).ToString(ArgumentParser.TimeSpanFormat)) { Align = Align.Left, Color = Red});
                    }
                    else if (task.InProgress())
                    {
                        grid.Children.Add(new Cell((DateTime.Now - task.TimeParams.Deadline).ToString(ArgumentParser.TimeSpanFormat)) { Align = Align.Left, Color = Yellow});
                    }
                    else if (!task.HasStarted())
                    {
                        grid.Children.Add(new Cell((DateTime.Now - task.TimeParams.Start).ToString(ArgumentParser.TimeSpanFormat)) { Align = Align.Left, Color = Green});
                    }
                }

                grid.Children.Add(new Cell("[" + task.Id.ToString() + "] " + task.Name) { Align = Align.Left, Color = DarkCyan });

                if (showDescriptionOption.value || showAllOption.value)
                {
                    grid.Children.Add(new Cell(task.Description) { Align = Align.Left, Color = White });
                }

                if (showChecklist.value || showAllOption.value)
                {
                    string checklist = "checklist:";
                    if (task.checklist.items.Count == 0)
                        checklist += " none";
                    else
                    {
                        checklist += "\n";
                        checklist += task.checklist.ToString(sortChecklistByDone.value);
                    }
                    grid.Children.Add(new Cell(checklist) { Align = Align.Left, Color = White });
                }

                if (showTimeLimits.value || showAllOption.value)
                {
                    ConsoleColor color = White;
                    if (task.TimeParams.enabled == false)
                        color = DarkGray;
                    if (task.TimeParams.Start == DateTime.MinValue)
                        grid.Children.Add(new Cell("start time not set") { Align = Align.Left, Color = color });
                    else
                        grid.Children.Add(new Cell("start:    " + task.TimeParams.Start.ToString("HH:mm dd.MM.yyyy")) { Align = Align.Left, Color = color });

                    if (task.TimeParams.Deadline == DateTime.MaxValue)
                        grid.Children.Add(new Cell("deadline not set") { Align = Align.Left, Color = color });
                    else
                        grid.Children.Add(new Cell("deadline: " + task.TimeParams.Deadline.ToString("HH:mm dd.MM.yyyy")) { Align = Align.Left, Color = color });
                }

                if (showRepeatParameters.value || showAllOption.value)
                {
                    ConsoleColor color = White;
                    if (task.TimeParams.repeat.enabled == false)
                        color = DarkGray;
                    grid.Children.Add(new Cell("repeat period: " + task.TimeParams.repeat.GetRepeatPeriod()) { Align = Align.Left, Color = color});
                    if (task.TimeParams.repeat.autorepeat)
                        grid.Children.Add(new Cell("autorepeat: " + task.TimeParams.repeat.autorepeat.ToString()) { Align = Align.Left, Color = DarkGreen });
                    else
                        grid.Children.Add(new Cell("autorepeat: " + task.TimeParams.repeat.autorepeat.ToString()) { Align = Align.Left, Color = DarkRed });
                }

                if (showChildrenOption.value || showAllOption.value)
                {
                    if (task.ChildIds.Count == 0)
                        grid.Children.Add(new Cell("no childs") { Align = Align.Left, Color = White });
                    else
                    {
                        List<Task> temp = taskManager.FindTask(task.ChildIds);
                        string taskString = "children:\n";
                        foreach (var val in temp)
                        {
                            taskString += "[" + val.Id.ToString() + "] " + val.Name;
                            if (val.Id != temp[temp.Count - 1].Id)
                                taskString += "\n";
                        }
                        grid.Children.Add(new Cell(taskString) { Align = Align.Left, Color = White });
                    }
                }

                if (showParentsOption.value || showAllOption.value)
                {
                    if (task.ParentIds.Count == 0)
                        grid.Children.Add(new Cell("no parents") { Align = Align.Left, Color = White });
                    else
                    {
                        List<Task> temp = taskManager.FindTask(task.ParentIds);
                        string taskString = "parents\n";
                        foreach (var val in temp)
                        {
                            taskString += "[" + val.Id.ToString() + "] " + val.Name;
                            if (val.Id != temp[temp.Count - 1].Id)
                                taskString += "\n";
                        }
                        grid.Children.Add(new Cell(taskString) { Align = Align.Left, Color = White, Stroke = LineThickness.Double });
                    }
                }

                if (showArchivedOption.value || showArchivedOnlyOption.value || showAllOption.value)
                {
                    if (task.Archived)
                        grid.Children.Add(new Cell("archived: " + task.Archived.ToString()) { Align = Align.Left, Color = DarkGreen });
                    else
                        grid.Children.Add(new Cell("archived: " + task.Archived.ToString()) { Align = Align.Left, Color = DarkRed });
                }

                grid.Columns.Add(cellWidth);
                mainGrid.Children.Add(grid);
            }

            for (int i = 0; i < columnCount; i++)
                mainGrid.Columns.Add(GridLength.Auto);
            
            doc.Children.Add(mainGrid);

            ConsoleRenderer.RenderDocument(doc);
        }
    }
}
