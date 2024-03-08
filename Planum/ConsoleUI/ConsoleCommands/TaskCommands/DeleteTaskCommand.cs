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
    public class DeleteTaskCommand: BaseCommand
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

        BoolSettingOption deleteArchivedOption = new BoolSettingOption("a", "delete all archived tasks", "", false);
            
        TaskManager taskManager;

        public DeleteTaskCommand(TaskManager taskManager) : base("delete", "delete tasks with specified parameters", "[options]")
        {
            this.taskManager = taskManager;

            idsOption = new TaskIdsOption(taskManager, "i", "define deleted task ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>(), true);
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

            options.Add(deleteArchivedOption);
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
                if (recursiveParentIdsOption.Used || recursiveParentNamesOption.Used)
                    if (!recursiveParents.Contains(task.Id))
                        result.Remove(task);
                if (recursiveChildIdsOption.Used || recursiveChildNamesOption.Used)
                    if (!recursiveChildren.Contains(task.Id))
                        result.Remove(task);
            }

            if (!options.Exists(x => x.Used))
            {
                ConsoleFormat.PrintError("action unspecified");
                return;
            }

            taskManager.DeleteTask(result.Select(x => x.Id).ToList());

            if (deleteArchivedOption.value)
            {
                List<int> archived_tasks = taskManager.FindTask().Where(x => x.Archived).Select(x => x.Id).ToList();
                taskManager.DeleteTask(archived_tasks);
            }

            ConsoleFormat.PrintSuccess("deleted tasks succesfully");
        }
    }
}
