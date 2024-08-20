using System.Collections.Generic;
using Planum.Config;

namespace Planum.Commands
{
    public class CreateNameOption : BaseOption<CreateCommandSettings>
    {
        public CreateNameOption(OptionInfo optionInfo, CommandConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref CreateCommandSettings result)
        {
            result.Name = args.Current;
            return true;
        }
    }
}
