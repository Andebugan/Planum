using System.Collections.Generic;
using Planum.Config;

namespace Planum.Commands
{
    public class DescriptionOption : BaseOption<TaskCommandSettings>
    {
        public DescriptionOption(OptionInfo optionInfo, CommandConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            result.Description = args.Current;
            return true;
        }
    }
}
