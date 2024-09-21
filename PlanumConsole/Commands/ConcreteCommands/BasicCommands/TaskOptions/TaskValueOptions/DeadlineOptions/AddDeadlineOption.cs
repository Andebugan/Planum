using Planum.Config;
using Planum.Logger;
using Planum.Model.Entities;

namespace Planum.Console.Commands.Task
{
    public class AddDeadlineOption: BaseOption<TaskCommandSettings>
    {
        public AddDeadlineOption(ILoggerWrapper logger, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(logger, optionInfo, commandConfig) { }
        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            foreach (var task in result.Tasks)
                task.Deadlines.Add(new Deadline());

            return true;
        }
    }
}
