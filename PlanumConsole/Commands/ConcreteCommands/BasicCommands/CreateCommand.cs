using Planum.Logger;
using Planum.Model.Entities;
using Planum.Model.Managers;

namespace Planum.Console.Commands.Task
{
    public class CreateCommand : BaseCommand<TaskCommandSettings>
    {
        TaskBufferManager TaskBufferManager { get; set; }

        public CreateCommand(TaskBufferManager taskBufferManager, CommandInfo commandInfo, List<BaseOption<TaskCommandSettings>> commandOptions, ILoggerWrapper logger) : base(commandInfo, commandOptions, logger)
        {
            TaskBufferManager = taskBufferManager;
        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing create command");
            var lines = new List<string>();
            var commandSettings = new TaskCommandSettings(TaskBufferManager);
            commandSettings.Tasks.Add(new PlanumTask());
            if (!ParseSettings(ref args, ref lines, ref commandSettings))
                return lines;

            var newTasks = commandSettings.Tasks;
            var validationResults = TaskBufferManager.Add(newTasks);
            if (validationResults.Any())
                foreach (var result in validationResults)
                    lines.Add(ConsoleSpecial.AddStyle(result.Message, foregroundColor: ConsoleInfoColors.Error));

            Logger.Log("Successfully added new task to buffer");
            return lines;
        }
    }
}
