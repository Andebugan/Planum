using System.Collections.Generic;
using Planum.Config;

namespace Planum.Console.Commands.Task
{
    public class TagRemoveOption: BaseOption<TaskCommandSettings>
    {
        public TagRemoveOption(OptionInfo optionInfo, CommandConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            result.Tags.Remove(args.Current);
            return true;
        }
    }
}
