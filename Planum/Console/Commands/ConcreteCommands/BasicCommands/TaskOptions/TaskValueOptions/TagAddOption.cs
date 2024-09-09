using System.Collections.Generic;
using Planum.Config;

namespace Planum.Console.Commands.Task
{
    public class TagAddOption: BaseOption<TaskCommandSettings>
    {
        public TagAddOption(OptionInfo optionInfo, CommandConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            result.Tags.Add(args.Current);
            return true;
        }
    }
}
