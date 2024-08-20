using System.Collections.Generic;
using System.Linq;
using Planum.Commands.Selector;
using Planum.Model.Managers;

namespace Planum.Commands
{
    public class CreateCommand : BaseCommand<CreateCommandSettings>
    {
        TaskBufferManager TaskBufferManager { get; set; }

        public CreateCommand(TaskBufferManager taskBufferManager, SelectorParser selectorParser, CommandInfo commandInfo, List<IOption<CreateCommandSettings>> commandOptions): base(selectorParser, commandInfo, commandOptions) => TaskBufferManager = taskBufferManager;

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            var lines = new List<string>();
            var commandSettings = new CreateCommandSettings(TaskBufferManager);
            if (!ParseSettings(ref args, ref lines, ref commandSettings))
                return lines;

            var validationResults = TaskBufferManager.Add(commandSettings.GetPlanumTask());
            if (validationResults.Any())
                foreach (var result in validationResults)
                    lines.Add(ConsoleSpecial.AddStyle(result.Message, foregroundColor: ConsoleInfoColors.Error));

            return lines;
        }
    }
}
