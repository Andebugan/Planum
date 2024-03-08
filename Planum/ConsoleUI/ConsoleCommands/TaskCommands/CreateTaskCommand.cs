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
    public class CreateTaskCommand: BaseCommand
    {
        TaskManager taskManager;
        BaseCommand showGridCommand;

        TaskNameOption nameOption;
        StringOption descriptionOption = new StringOption("d", "define task description, ", "[description]", "");
        TaskIdsOption parentIdsOption;
        TaskNamesOption parentNamesOption;
        TaskIdsOption childIdsOption;
        TaskNamesOption childNamesOption;
        BoolSettingOption archivedOption = new BoolSettingOption("a", "if used, created task will be archived", "", false);
        DateTimeOption startTimeOption = new DateTimeOption("st", "define task start time, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.MinValue);
        DateTimeOption deadlineOption = new DateTimeOption("dl", "define task deadline, time format: " + ArgumentParser.DateTimeFormat, "[datetime]", DateTime.MaxValue, defaultEndOfDay:true);
        BoolSettingOption autoRepeatOption = new BoolSettingOption("ar", "enable autorepeat for task", "", false);

        IntValueOption repeatYearsOption = new IntValueOption("ry", "define repeat period years value", "[years]", 0);
        IntValueOption repeatMonthsOption = new IntValueOption("rm", "define repeat period month value", "[months]", 0);
        TimespanOption repeatTimeSpanOption = new TimespanOption("rt", "define repeat period timespan in days, hours and minute values for repeating task, value format: " + ArgumentParser.GetTimeSpanFormat(), "[timespan]", TimeSpan.Zero);

        StringListOption checklistOption = new StringListOption("cl", "add items to checklist", "[item_1] " + ArgumentParser.NameSeparator + " [item_2] ...", new List<string>());
        BoolSettingOption resetChecklistOnCompleteOption = new BoolSettingOption("rcc", "reset checklist on complete, checked values are reset on complete and can be restored only with undo command", "", false);

        BoolSettingOption showResultOption = new BoolSettingOption("sr", "shows created task", "", false);

        public CreateTaskCommand(TaskManager taskManager, BaseCommand showGridCommand): base("create", "creates new task", "[options]")
        {
            this.taskManager = taskManager;
            this.showGridCommand = showGridCommand;

            nameOption = new TaskNameOption(taskManager, "n", "define task name", "[name]", "", false);
            options.Add(nameOption);

            options.Add(descriptionOption);

            parentIdsOption = new TaskIdsOption(taskManager, "p", "define parent ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(parentIdsOption);

            parentNamesOption = new TaskNamesOption(taskManager, "pn", "define parent names", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(parentNamesOption);

            childIdsOption = new TaskIdsOption(taskManager, "c", "define children ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>());
            options.Add(childIdsOption);

            childNamesOption = new TaskNamesOption(taskManager, "cn", "define children names", "[name_1] " + ArgumentParser.NameSeparator + " [name_2] ...", new List<int>());
            options.Add(childNamesOption);

            options.Add(archivedOption);
            options.Add(startTimeOption);
            options.Add(deadlineOption);
            options.Add(autoRepeatOption);
            options.Add(repeatYearsOption);
            options.Add(repeatMonthsOption);
            options.Add(repeatTimeSpanOption);

            options.Add(checklistOption);
            options.Add(showResultOption);
            options.Add(resetChecklistOnCompleteOption);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            bool repeatEnabled = false;
            if (repeatTimeSpanOption.Used || repeatMonthsOption.Used || repeatYearsOption.Used)
                repeatEnabled = true;
            RepeatParams repeatParams = new RepeatParams(repeatEnabled, autoRepeatOption.value, repeatYearsOption.value, repeatMonthsOption.value, repeatTimeSpanOption.value);

            bool timeParamsEnabled = false;
            if (startTimeOption.Used == true || deadlineOption.Used == true)
                timeParamsEnabled = true;

            Checklist checklist = new Checklist();
            foreach (var item in checklistOption.value)
                checklist.AddItem(item);
            checklist.ResetOnComplete = resetChecklistOnCompleteOption.value;

            TimeParams timeParams = new TimeParams(timeParamsEnabled, startTimeOption.value, deadlineOption.value, repeatParams);

            parentIdsOption.value = parentIdsOption.value.Concat(parentNamesOption.value).Distinct().ToList();
            childIdsOption.value = childIdsOption.value.Concat(childNamesOption.value).Distinct().ToList();

            Task task = new Task(-1, nameOption.value, descriptionOption.value, parentIdsOption.value, childIdsOption.value, 
                timeParams, archivedOption.value, checklist);

            string result = taskManager.ValidateTask(task);

            if (result != "")
            {
                ConsoleFormat.PrintError(result);
                return;
            }

            int id = taskManager.CreateTask(task);
            if (!showResultOption.value)
                ConsoleFormat.PrintSuccess("new task with id: " + id + " created succesfully");
            else
            {
                showGridCommand.Execute(new List<string> {"show", ArgumentParser.CommandDelimeter + "i", id.ToString(), ArgumentParser.CommandDelimeter + "all" });
            }
        }
    }
}
