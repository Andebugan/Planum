using System.Collections.Generic;
using Planum.Config;

namespace Planum.Console.Commands.Task
{
    public class AddDeadlineOption: BaseOption<TaskCommandSettings>
    {
        public AddDeadlineOption(OptionInfo optionInfo, CommandConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            result.CommitDeadline();
            return true;
        }
    }
}
