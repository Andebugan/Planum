using Planum.Config;
using Planum.Model.Entities;

namespace Planum.Console.Commands.Task
{
    public class AddDeadlineOption: BaseOption<TaskCommandSettings>
    {
        public AddDeadlineOption(OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            foreach (var task in result.Tasks)
                task.Deadlines.Add(new Deadline());

            return true;
        }
    }
}
