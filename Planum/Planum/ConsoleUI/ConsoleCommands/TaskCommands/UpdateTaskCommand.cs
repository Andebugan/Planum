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
    public class UpdateTaskCommand: BaseCommand
    {
        TaskIdsOption idsOption;
        TaskNameOption nameOption;
        StringOption descriptionOption = new StringOption("d", "defines new task description", "[description]", "", defaultVal:"");
        TaskIdsOption parentIdsOption;
        TaskIdsOption addParentIdsOption;
        TaskIdsOption removeParentIdsOption;

        TaskNamesOption parentNamesOption;
        TaskNamesOption addParentNamesOption;
        TaskNamesOption removeParentNamesOption;

        TaskIdsOption childIdsOption;
        TaskIdsOption addChildIdsOption;
        TaskIdsOption removeChildIdsOption;

        TaskNamesOption childNamesOption;
        TaskNamesOption addChildNamesOption;
        TaskNamesOption removeChildNamesOption;

        BoolValueOption archivedOption = new BoolValueOption("a", "change task archived status", "[y/n]", false);
        BoolValueOption enableTimedOption = new BoolValueOption("t", "enable/disable time parameters", "[y/n]", false);
        BoolValueOption enableRepeatOption = new BoolValueOption("r", "enable/disable repeat for task", "[y/n]", false);

        DateTimeOption startTimeOption = new DateTimeOption("st", "define new start time, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.MinValue, defaultVal:DateTime.MinValue);
        DateTimeOption deadlineOption = new DateTimeOption("dl", "define new deadline, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.MaxValue, defaultVal:DateTime.MaxValue, defaultEndOfDay:true);

        BoolValueOption autorepeatOption = new BoolValueOption("ar", "enable/disable autorepeat", "[y/n]", false);

        IntValueOption repeatYearsOption = new IntValueOption("ry", "define new repeat period years value", "[years]", 0, defaultVal:0);
        IntValueOption repeatMonthsOption = new IntValueOption("rm", "define new repeat period month value", "[months]", 0, defaultVal:0);
        TimespanOption repeatTimeSpanOption = new TimespanOption("rt", "define new repeat period timespan in days, hours and minute values for repeating task, value format: " + ArgumentParser.GetTimeSpanFormat(), "[timespan]", TimeSpan.Zero, defaultVal:TimeSpan.Zero);

        StringListOption checklistOption = new StringListOption("cl", "define new checklist items", "[item_1] " + ArgumentParser.NameSeparator + " [item_2] ...", new List<string>(), defaultVal:new List<string>());
        StringListOption addChecklistOption = new StringListOption("acli", "add items to checklist", "[item_1] " + ArgumentParser.NameSeparator + " [item_2] ...", new List<string>());

        IntListValueOption removeChecklistOption = new IntListValueOption("rcli", "remove items from checklist", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
        IntListValueOption checkCheklistOption = new IntListValueOption("cli", "check checklist items", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
        IntListValueOption uncheckChecklistOption = new IntListValueOption("ucli", "uncheck checklist items", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());

        BoolSettingOption showResultOption = new BoolSettingOption("sr", "shows updated tasks", "", false);
        BoolSettingOption resetChecklistOnComplete = new BoolSettingOption("rcc", "reset checklist on complete, checked values are reset on complete and can be restored only with undo command", "", false);

        TaskManager taskManager;
        BaseCommand showGridCommand;

        public UpdateTaskCommand(TaskManager taskManager, BaseCommand showGridCommand) : base("update", "updates task with given values," +
            " note that updating the same parameter in several different ways (replace, add, remove, ect.) in the same order in which these options were given",
            "[options]")
        {
            this.taskManager = taskManager;
            this.showGridCommand = showGridCommand;

            idsOption = new TaskIdsOption(taskManager, "i", "defines updated task ids, if given more than one will update all with given parameters", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", taskManager.FindTask().Select(i => i.Id).ToList());
            options.Add(idsOption);

            nameOption = new TaskNameOption(taskManager, "n", "defines new task name", "[name]", "");
            options.Add(nameOption);

            options.Add(descriptionOption);

            parentIdsOption = new TaskIdsOption(taskManager, "p", "defines new parent ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>(), defaultVal:new List<int>());
            options.Add(parentIdsOption);

            addParentIdsOption = new TaskIdsOption(taskManager, "ap", "defines ids of parents that will be added to existing task parents", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(addParentIdsOption);

            removeParentIdsOption = new TaskIdsOption(taskManager, "rp", "defines ids of parents that will be removed from existing task parents", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(removeParentIdsOption);


            parentNamesOption = new TaskNamesOption(taskManager, "pn", "defines new parent names", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>(), defaultVal: new List<string>());
            options.Add(parentNamesOption);

            addParentNamesOption = new TaskNamesOption(taskManager, "apn", "defines ids of parents that will be added to existing task parents", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(addParentNamesOption);

            removeParentNamesOption = new TaskNamesOption(taskManager, "rpn", "defines ids of parents that will be removed from existing task parents", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(removeParentNamesOption);


            childIdsOption = new TaskIdsOption(taskManager, "c", "defines new child ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>(), defaultVal: new List<int>());
            options.Add(childIdsOption);

            addChildIdsOption = new TaskIdsOption(taskManager, "aс", "defines ids of children that will be added to existing task children", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(addChildIdsOption);

            removeChildIdsOption = new TaskIdsOption(taskManager, "rс", "defines ids of children that will be removed from existing task children", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(removeChildIdsOption);


            childNamesOption = new TaskNamesOption(taskManager, "cn", "defines new child names", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>(), defaultVal:new List<string>());
            options.Add(childNamesOption);

            addChildNamesOption = new TaskNamesOption(taskManager, "acn", "defines ids of children that will be added to existing task children", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(addChildNamesOption);

            removeChildNamesOption = new TaskNamesOption(taskManager, "rcn", "defines ids of children that will be removed from existing task children", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(removeChildNamesOption);

            options.Add(archivedOption);
            options.Add(enableTimedOption);
            options.Add(enableRepeatOption);

            options.Add(startTimeOption);
            options.Add(deadlineOption);
            options.Add(autorepeatOption);

            options.Add(repeatYearsOption);
            options.Add(repeatMonthsOption);
            options.Add(repeatTimeSpanOption);

            options.Add(checklistOption);
            options.Add(addChecklistOption);
            options.Add(removeChecklistOption);
            options.Add(checkCheklistOption);
            options.Add(uncheckChecklistOption);

            options.Add(showResultOption);
            options.Add(resetChecklistOnComplete);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            if (startTimeOption.value > deadlineOption.value)
            {
                ConsoleFormat.PrintError("start time can't be later then deadline");
                return;
            }

            List<Task> updatedTasks = taskManager.FindTask(idsOption.value);

            if (!idsOption.Used)
            {
                ConsoleFormat.PrintWarning("confirm updating all tasks [y/n]: ", false);
                string input = Console.ReadLine();
                if (input != "y" && input != "n")
                {
                    ConsoleFormat.PrintError("incorrect input");
                    return;
                }
                if (input == "n")
                    return;
            }

            List<Task> newTasks = new List<Task>();

            Dictionary<int, bool> archivedUpdate = new Dictionary<int, bool>();

            foreach (var task in updatedTasks)
            {
                foreach (var option in usedOptions)
                {
                    if (option == nameOption)
                        task.Name = nameOption.value;
                    if (option == descriptionOption)
                        task.Description = descriptionOption.value;

                    if (option == parentIdsOption)
                        task.ParentIds = parentIdsOption.value.Concat(parentNamesOption.value).Distinct().ToList();
                    if (option == addParentIdsOption)
                        task.ParentIds = task.ParentIds.Concat(addParentIdsOption.value).Distinct().ToList();
                    if (option == removeParentIdsOption)
                        task.ParentIds = task.ParentIds.Where(x => !removeParentIdsOption.value.Contains(x)).Distinct().ToList();
                    if (option == parentNamesOption)
                        task.ParentIds = parentIdsOption.value.Concat(parentNamesOption.value).Distinct().ToList();
                    if (option == addParentNamesOption)
                        task.ParentIds = task.ParentIds.Concat(addParentNamesOption.value).Distinct().ToList();
                    if (option == removeParentNamesOption)
                        task.ParentIds = task.ParentIds.Concat(removeParentNamesOption.value).Distinct().ToList();

                    if (option == childIdsOption)
                        task.ChildIds = childIdsOption.value.Concat(childNamesOption.value).Distinct().ToList();
                    if (option == addChildIdsOption)
                        task.ChildIds = task.ChildIds.Concat(addChildIdsOption.value).Distinct().ToList();
                    if (option == removeParentIdsOption)
                        task.ChildIds = task.ChildIds.Where(x => !removeChildIdsOption.value.Contains(x)).Distinct().ToList();
                    if (option == parentNamesOption)
                        task.ChildIds = childIdsOption.value.Concat(childNamesOption.value).Distinct().ToList();
                    if (option == addParentNamesOption)
                        task.ChildIds = task.ChildIds.Concat(addChildNamesOption.value).Distinct().ToList();
                    if (option == removeParentNamesOption)
                        task.ChildIds = task.ChildIds.Concat(removeChildNamesOption.value).Distinct().ToList();

                    if (option == archivedOption)
                        archivedUpdate.Add(task.Id, archivedOption.value);
                    if (option == startTimeOption)
                        task.TimeParams.Start = startTimeOption.value;
                    if (option == deadlineOption)
                        task.TimeParams.Deadline = deadlineOption.value;

                    if (startTimeOption.value > deadlineOption.value)
                    {
                        ConsoleFormat.PrintError("start time can't be later then deadline");
                        return;
                    }

                    if (option == autorepeatOption)
                        task.TimeParams.repeat.autorepeat = autorepeatOption.value;

                    if (option == repeatYearsOption)
                        task.TimeParams.repeat.years = repeatYearsOption.value;
                    if (option == repeatMonthsOption)
                        task.TimeParams.repeat.months = repeatMonthsOption.value;
                    if (option == repeatTimeSpanOption)
                        task.TimeParams.repeat.custom = repeatTimeSpanOption.value;

                    if (startTimeOption.Used || deadlineOption.Used)
                        task.TimeParams.enabled = true;

                    if (repeatYearsOption.Used || repeatMonthsOption.Used || repeatTimeSpanOption.Used)
                        task.TimeParams.repeat.enabled = true;

                    if (option == enableTimedOption)
                        task.TimeParams.enabled = enableTimedOption.value;
                    if (option == enableRepeatOption)
                        task.TimeParams.repeat.enabled = enableRepeatOption.value;

                    if (option == checklistOption)
                    {
                        Checklist checklist = new Checklist();
                        foreach (var val in checklistOption.value)
                            checklist.AddItem(val);
                        checklist.items = checklist.items.Distinct().ToList();
                        task.checklist = checklist;
                    }

                    if (option == addChecklistOption)
                    {
                        foreach (var val in addChecklistOption.value)
                            task.checklist.AddItem(val);
                        task.checklist.items = task.checklist.items.Distinct().ToList();
                    }

                    if (option == removeChecklistOption)
                    {
                        if (removeChecklistOption.value.Exists(x => x < 0 || x >= task.checklist.items.Count))
                        {
                            ConsoleFormat.PrintError("incorrect checklist item index at task: " + task.Id.ToString());
                            return;
                        }

                        Checklist checklist = new Checklist();
                        for (int i = 0; i < task.checklist.items.Count; i++)
                        {
                            if (!removeChecklistOption.value.Contains(i))
                                checklist.AddItem(task.checklist.items[i].description, task.checklist.items[i].done);
                        }
                        task.checklist = checklist;
                    }

                    if (option == checkCheklistOption)
                    {
                        if (checkCheklistOption.value.Exists(x => x < 0 || x >= task.checklist.items.Count))
                        {
                            ConsoleFormat.PrintError("incorrect checklist item index at task: " + task.Id.ToString());
                            return;
                        }

                        foreach (var val in checkCheklistOption.value)
                        {
                            task.checklist.items[val].done = true;
                        }
                    }

                    if (option == uncheckChecklistOption)
                    {
                        if (uncheckChecklistOption.value.Exists(x => x < 0 || x >= task.checklist.items.Count))
                        {
                            ConsoleFormat.PrintError("incorrect checklist item index at task: " + task.Id.ToString());
                            return;
                        }

                        foreach (var val in uncheckChecklistOption.value)
                        {
                            task.checklist.items[val].done = false;
                        }
                    }

                    if (option == resetChecklistOnComplete)
                    {
                        task.checklist.ResetOnComplete = resetChecklistOnComplete.value;
                    }
                }

                string result = taskManager.ValidateTask(task);
                
                if (result != "")
                {
                    ConsoleFormat.PrintError(result);
                    return;
                }

                newTasks.Add(task);
            }

            List<string> showStr = new List<string>();
            showStr.Add("show");
            showStr.Add(ArgumentParser.CommandDelimeter + "i");

            foreach (var task in newTasks)
            {
                taskManager.UpdateTask(task);
                showStr.Add(task.Id.ToString());
            }

            foreach (var task in newTasks.Where(x => archivedUpdate.Keys.Contains(x.Id)))
            {
                taskManager.UpdateArchived(task, !archivedUpdate[task.Id]);
            }

            showStr.Add(ArgumentParser.CommandDelimeter + "all");

            if (!showResultOption.value)
                ConsoleFormat.PrintSuccess("tasks updated succesfully");
            else
            {
                showGridCommand.Execute(showStr);
            }
        }
    }
}
